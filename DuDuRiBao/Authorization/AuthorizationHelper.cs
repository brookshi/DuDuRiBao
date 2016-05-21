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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.UI.Xaml;
using Brook.DuDuRiBao.Utils;
using Brook.DuDuRiBao.ViewModels;
using Brook.DuDuRiBao.Common;
using XPHttp;
using Brook.DuDuRiBao.Models;

namespace Brook.DuDuRiBao.Authorization
{
    public static class AuthorizationHelper
    {
        public static bool IsLogin = false;
        private readonly static Dictionary<LoginType, IAuthorize> Authorizations = new Dictionary<LoginType, IAuthorize>();

        static AuthorizationHelper()
        {
            var currentAssembly = Application.Current.GetType().GetTypeInfo().Assembly;

            var authorizeTypes = currentAssembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(IAuthorize))).ToList();

            authorizeTypes.ForEach(o => Authorizations[GetLoginType(o)] = GetAuthorization(o));
        }

        private static LoginType GetLoginType(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute<AuthoAttribution>().LoginType;
        }

        private static IAuthorize GetAuthorization(TypeInfo typeInfo)
        {
            return (IAuthorize)typeInfo.GetDeclaredField("Instance").GetValue(null);
        }

        public static async Task<bool> AutoLogin()
        {
            if(!(await AutoLoginZhiHu()))
            {
                return await AnonymousLogin();
            }

            return true;
        }

        public static async Task<bool> AutoLoginZhiHu()
        {
            if (IsLogin)
                return true;

            string msg = string.Empty;
            var loginType = StorageUtil.StorageInfo.LoginType;
            if (!CheckLoginType(loginType, out msg))
                return false;

            var authorizer = Authorizations[loginType];
            if (!authorizer.IsAuthorized)
                return false;

            if (StorageUtil.StorageInfo.IsZhiHuAuthoVaild())
            {
                IsLogin = true;
                SetHttpAuthorization();
                return true;
            }
            else if (Authorizations[loginType].LoginData != null)
            {
                return await LoginZhiHu(loginType);
            }

            return false;
        }

        public static async Task<bool> AnonymousLogin()
        {
            var key = LoginKeyProvider.GetAnonymousLoginKey();
            LoginToken loginToken = await DataRequester.AnonymousLogin(key);
            if (loginToken == null)
                return false;

            StorageUtil.StorageInfo.ZhiHuAuthoInfo = new RiBaoAuthoInfo() { AnonymousLoginToken = loginToken.Access_Token };
            SetHttpAuthorization();

            return true;
        }

        public static async void Login(LoginType loginType, Action<bool> loginCallback)
        {
            if (IsLogin)
                return;

            if (loginCallback == null)
                loginCallback = isSuccess => { };

            string msg = string.Empty;
            if (!CheckLoginType(loginType, out msg))
            {
                loginCallback(false);
            }
            var authorizer = Authorizations[loginType];

            var loginSuccess = false;
            if (authorizer.IsAuthorized && Authorizations[loginType].LoginData != null)
            {
                loginSuccess = await LoginZhiHu(loginType);
                loginCallback(loginSuccess);
            }
            else
            {
                try
                {
                    authorizer.Login(async (isSuccess, res) =>
                    {
                        if (isSuccess)
                        {
                            loginSuccess = await LoginZhiHu(loginType);
                            loginCallback(loginSuccess);
                        }
                    });
                }
                catch (Exception)
                {
                    loginCallback(loginSuccess);
                }
            }
        }

        private static async Task<bool> LoginZhiHu(LoginType loginType)
        {
            var zhiHuAuthoData = await DataRequester.LoginUsingWeibo(Authorizations[loginType].LoginData);
            if (zhiHuAuthoData == null)
            {
                return false;
            }
            StorageUtil.StorageInfo.LoginType = loginType;
            StorageUtil.StorageInfo.ZhiHuAuthoInfo = zhiHuAuthoData;
            StorageUtil.UpdateStorageInfo();

            IsLogin = true;
            SetHttpAuthorization();
            return true;
        }

        private static void SetHttpAuthorization()
        {
            XPHttpClient.DefaultClient.HttpConfig.SetAuthorization("Bearer", StorageUtil.StorageInfo.ZhiHuAuthoInfo.access_token ?? StorageUtil.StorageInfo.ZhiHuAuthoInfo.AnonymousLoginToken);
        }

        public static void Logout()
        {
            IsLogin = false;
            string msg;
            var loginType = StorageUtil.StorageInfo.LoginType;

            if (CheckLoginType(loginType, out msg))
            {
                Authorizations[loginType].Logout();
            }
        }

        private static bool CheckLoginType(LoginType loginType, out string msg)
        {
            msg = string.Empty;
            if (loginType == LoginType.None)
            {
                msg = StringUtil.GetString("NoneLoginType");
                return false;
            }

            if (!Authorizations.ContainsKey(loginType))
            {
                msg = StringUtil.GetString("NotSupportLoginType");
                return false;
            }

            return true;
        }
    }
}
