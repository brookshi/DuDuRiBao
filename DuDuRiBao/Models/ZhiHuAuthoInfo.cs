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

using System.Collections.Generic;

namespace Brook.DuDuRiBao.Models
{
    public class RiBaoAuthoInfo
    {
        public string access_token { get; set; }

        public List<string> bound_services { get; set; }

        public string name { get; set; }

        public string avatar { get; set; }

        public string AnonymousLoginToken { get; set; }
    }

    public class Cookie
    {
        public string Q_C0 { get; set; }
        public string Z_C0 { get; set; }
    }
    public class ZhiHuSignInfo
    {
        public long User_Id { get; set; }
        public string Uid { get; set; }
        public string Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Token_Type { get; set; }
        public Cookie Cookie { get; set; }
        public string Refresh_Token { get; set; }
    }

    public class ZhiHuTokenInfo
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Code { get; set; }
        public int Expires_In { get; set; }
    }

    public class Captcha
    {
        public bool Show_Captcha { get; set; }
        public string Img_Base64 { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class CaptchaChecked
    {
        public Error Error { get; set; }
        public bool Success { get; set; }
    }

    public class ZhiHuAuthInfo
    {
        public string AuthorizationCode { get; set; }
    }
}
