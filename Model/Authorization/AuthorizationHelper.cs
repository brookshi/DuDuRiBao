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

            var authorizeTypes = currentAssembly.DefinedTypes.Where(type => type.BaseType == typeof(AuthorizationBase)).ToList();

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

        public static IAuthorize GetAuthorization(LoginType loginType)
        {
            return Authorizations.ContainsKey(loginType) ? Authorizations[loginType] : null;
        }

        public static async Task<bool> AutoLogin()
        {
            if(!(await Login()))
            {
                await AnonymousLogin();
                return false;
            }

            return true;
        }

        public static async Task<bool> Login()
        {
            if (IsLogin)
                return true;

            string msg = string.Empty;
            var loginType = StorageInfo.Instance.LoginType;
            if (!CheckLoginType(loginType, out msg))
                return false;

            var authorizer = Authorizations[loginType];
            if (!authorizer.IsAuthorized)
                return false;

            if (StorageInfo.Instance.IsZhiHuAuthoVaild())
            {
                IsLogin = true;
                SetHttpAuthorization();
                return true;
            }
            else if (Authorizations[loginType].TokenInfo != null)
            {
                await Authorizations[loginType].Login(null, autho=>UpdateLoginInfo(loginType, autho));
                return true;
            }

            return false;
        }

        public static async Task<bool> AnonymousLogin()
        {
            var key = LoginKeyProvider.GetAnonymousLoginKey();
            LoginToken loginToken = await DataRequester.AnonymousLogin(key);
            if (loginToken == null)
                return false;

            StorageInfo.Instance.ZhiHuAuthoInfo = new RiBaoAuthoInfo() { AnonymousLoginToken = loginToken.Access_Token };
            SetHttpAuthorization();

            return true;
        }

        public static void Login(LoginType loginType, ZhiHuLoginInfo loginInfo, Action<bool> loginCallback)
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
            Authorizations[loginType].Login(loginInfo, autho => {
                var isSuccess = autho != null;
                if (isSuccess)
                    UpdateLoginInfo(loginType, autho);
                loginCallback?.Invoke(isSuccess);
            });
        }

        private static void UpdateLoginInfo(LoginType loginType, RiBaoAuthoInfo info)
        {
            StorageInfo.Instance.LoginType = loginType;
            StorageInfo.Instance.ZhiHuAuthoInfo = info;
            Storager.UpdateStorageInfo();

            IsLogin = true;
            SetHttpAuthorization();
        }

        private static void SetHttpAuthorization()
        {
            XPHttpClient.DefaultClient.HttpConfig.SetAuthorization("Bearer", StorageInfo.Instance.ZhiHuAuthoInfo.access_token ?? StorageInfo.Instance.ZhiHuAuthoInfo.AnonymousLoginToken);
        }

        private static void ClearHttpAuthorization()
        {
            XPHttpClient.DefaultClient.HttpConfig.SetAuthorization("Bearer", "");
        }

        public static async Task Logout()
        {
            IsLogin = false;
            string msg;
            var loginType = StorageInfo.Instance.LoginType;
            StorageInfo.Instance.LoginType = LoginType.None;
            StorageInfo.Instance.ZhiHuAuthoInfo = null;
            Storager.UpdateStorageInfo();
            ClearHttpAuthorization();

            if (CheckLoginType(loginType, out msg))
            {
                Authorizations[loginType].Logout();
            }
            await AnonymousLogin();
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
