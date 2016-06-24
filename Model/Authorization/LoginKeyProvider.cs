#region License
//   Copyright 2015 Brook Shi
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Net;

namespace Brook.DuDuRiBao.Authorization
{
    public class LoginKeyProvider
    {
        public const string ClientId = "ee61ede15113741dca8bca59479ce6";
        private const string ZhiHuPacketName = "com.zhihu.android";
        private const string GrantType = "password";
        private const string Secret = "8b735aeaecebc6aaf1f0ece3afdc8b";

        private static byte[] MD5_Key = { 131, 68, 122, 224, 28, 16, 38, 162, 26, 108, 95, 245, 31, 141, 183, 236 };
        private static byte[] AES_Key = { 82, 138, 106, 79, 144, 17, 230, 153, 3, 192, 216, 240, 10, 243, 105, 51 };
        private static byte[] Param = { 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string GetAnonymousLoginKey()
        {
            byte[] radomBytes16 = GenerateRandomByte(16);
            byte[] radomByte4 = GenerateRandomByte(4);
            int time = (int)(DateTime.Now - new DateTime(1970,1,1)).TotalSeconds;
            
            byte[] timeBytes = BitConverter.GetBytes(time);
            byte[] timeBytes1 = timeBytes.Concat(radomByte4).ToArray();
            byte[] timeBytes2 = timeBytes1.Concat(Param).ToArray(); 

            byte[] arrayOfByte = AESHandle(timeBytes2, radomBytes16);

            byte[] arrayOfByte1 = radomBytes16.Concat(arrayOfByte).ToArray();
            byte[] arrayOfByte2 = MD5Handle(arrayOfByte1);
            byte[] arrayOfByte3 = arrayOfByte2.Concat(radomBytes16).ToArray();
            byte[] arrayOfByte4 = arrayOfByte3.Concat(arrayOfByte).ToArray();
            string rst = CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.CreateFromByteArray(arrayOfByte4));

            return rst;
        }

        public static string GetZhiHuLoginKey(string name, string password)
        {
            name = name.StartsWith("+86") ? WebUtility.UrlEncode(name) : name;
            var timestamp = ((int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            var signature = GetZhiHuSignatureKey(timestamp);
            return string.Format($"client_id={ClientId}&grant_type={GrantType}&password={password}&signature={signature }&source={ZhiHuPacketName}&timestamp={timestamp}&username={name}");
        }

        static string GetZhiHuSignatureKey(string timestamp)
        {
            var param = GrantType + ClientId + ZhiHuPacketName + timestamp.ToString();
            var signature = SHA1Handle(CryptographicBuffer.ConvertStringToBinary(param, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToHexString(signature);
        }

        static byte[] MD5Handle(IBuffer data)
        {
            IBuffer key = CryptographicBuffer.CreateFromByteArray(MD5_Key);
            var signature = HMACHandle(data, key, "HMAC_MD5");
            byte[] rst = new byte[signature.Length];
            CryptographicBuffer.CopyToByteArray(signature, out rst);
            return rst;
        }

        static byte[] MD5Handle(byte[] data)
        {
            return MD5Handle(CryptographicBuffer.CreateFromByteArray(data));
        }

        static IBuffer SHA1Handle(IBuffer data)
        {
            IBuffer keyBuf = CryptographicBuffer.ConvertStringToBinary(Secret, BinaryStringEncoding.Utf8);
            return HMACHandle(data, keyBuf, "HMAC_SHA1");
        }

        static IBuffer HMACHandle(IBuffer data, IBuffer key, string hmacType)
        {
            MacAlgorithmProvider hmacAlgorithm = MacAlgorithmProvider.OpenAlgorithm(hmacType);
            CryptographicKey hmacKey = hmacAlgorithm.CreateKey(key);

            return CryptographicEngine.Sign(hmacKey, data);
        }

        static byte[] AESHandle(byte[] data, byte[] paramAlgorithm)
        {
            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");
            IBuffer key = CryptographicBuffer.CreateFromByteArray(AES_Key);
            CryptographicKey cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key);
            var iv = CryptographicBuffer.CreateFromByteArray(paramAlgorithm);
            
            var encrypted = CryptographicEngine.Encrypt(cryptoKey, CryptographicBuffer.CreateFromByteArray(data), iv);
            byte[] rst = new byte[encrypted.Length];
            CryptographicBuffer.CopyToByteArray(encrypted, out rst);
            return rst;
        }

        static byte[] GenerateRandomByte(uint length)
        {
            byte[] bytes = new byte[length];
            CryptographicBuffer.CopyToByteArray(CryptographicBuffer.GenerateRandom(length), out bytes);
            return bytes;
        }

        static sbyte[] ByteToSByte(byte[] bytes)
        {
            sbyte[] sbytes = new sbyte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 127)
                    sbytes[i] = (sbyte)(bytes[i] - 256);
                else
                    sbytes[i] = (sbyte)bytes[i];
            }

            return sbytes;
        }

        static byte[] SByteToByte(sbyte[] sbytes)
        {
            byte[] bytes = new byte[sbytes.Length];

            for (int i = 0; i < sbytes.Length; i++)
            {
                if (sbytes[i] < 0)
                    bytes[i] = (byte)(sbytes[i] + 256);
                else
                    bytes[i] = (byte)sbytes[i];
            }

            return bytes;
        }
    }
}
