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

using Brook.DuDuRiBao.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Utils;

namespace Brook.DuDuRiBao.Authorization
{
    [AuthoAttribution(LoginType.ZhiHu)]
    public class ZhiHuAuthorization : AuthorizationBase
    {
        public static readonly ZhiHuAuthorization Instance = new ZhiHuAuthorization();

        public override bool IsAuthorized
        {
            get
            {
                return TokenInfo != null && !string.IsNullOrEmpty(TokenInfo.access_token) && DateTime.Now < TokenInfo.LastAuthoDate.AddSeconds(TokenInfo.Expires_In);
            }
        }

        public override async Task Login(ZhiHuLoginInfo info, Action<RiBaoAuthoInfo> loginCallback)
        {
            if(IsAuthorized)
            {
                loginCallback?.Invoke(await DataRequester.LoginUsingZhiHu(TokenInfo));
                return;
            }

            if (info == null)
            {
                loginCallback?.Invoke(null);
                return;
            }

            if (!await CheckCaptcha(info.Captcha))
            {
                loginCallback?.Invoke(null);
                return;
            }

            var signInfo = await DataRequester.ZhiHuLogin(info.UserName, info.Password);
            if (!CheckError(signInfo))
            {
                loginCallback?.Invoke(null);
                return;
            }

            var zhiHuAutoInfo = await DataRequester.GetZhiHuAuthorization(signInfo);
            if (!CheckError(zhiHuAutoInfo))
            {
                loginCallback?.Invoke(null);
                return;
            }

            TokenInfo = await DataRequester.GetZhiHuToken(zhiHuAutoInfo);
            if (TokenInfo == null || string.IsNullOrEmpty(TokenInfo.access_token))
            {
                PopupMessage.DisplayMessage(StringUtil.GetString("LoginFailed") + TokenInfo?.Data ?? "");
                loginCallback?.Invoke(null);
                return;
            }
            TokenInfo.LastAuthoDate = DateTime.Now;
            TokenInfo.source = LoginType.ZhiHu.Convert();
            StoreTokenInfo();
            var autoInfo = await DataRequester.LoginUsingZhiHu(TokenInfo);
            loginCallback?.Invoke(autoInfo);
        }

        private bool CheckError(ErrorBase error)
        {
            if (error == null)
            {
                PopupMessage.DisplayMessageInRes("LoginFailed");
                return false;
            }
            if (error.Error != null)
            {
                if (string.IsNullOrEmpty(error.Error.Message))
                    PopupMessage.DisplayMessageInRes("LoginFailed");
                else
                    PopupMessage.DisplayMessage(error.Error.Message);

                return false;
            }

            return true;
        }

        private async Task<bool> CheckCaptcha(string captcha)
        {
            var captchaChecked = await DataRequester.CheckCaptcha(captcha);
            if (captchaChecked != null && captchaChecked.Success)
                return true;

            if (captchaChecked != null && captchaChecked.Error != null && !string.IsNullOrEmpty(captchaChecked.Error.Message))
                PopupMessage.DisplayMessage(captchaChecked.Error.Message);
            else
                PopupMessage.DisplayMessageInRes("CaptchaCheckFailed");

            return false;
        }
    }
}
