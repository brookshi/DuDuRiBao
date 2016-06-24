using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using System.Threading.Tasks;
using WeiboSDKForWinRT;

namespace Brook.DuDuRiBao.Authorization
{
    [AuthoAttribution(LoginType.Sina)]
    public class SinaAuthorization : AuthorizationBase
    {
        public static readonly SinaAuthorization Instance = new SinaAuthorization();

        protected override string LoginDataKey { get { return "SinaTokenInfo"; } }

        private ClientOAuth _oauth = new ClientOAuth();

        static SinaAuthorization()
        {
            SdkData.AppKey = "2615126550";
            SdkData.AppSecret = "d8f2b0dc26390ddb844f45b5a6f69328";
            SdkData.RedirectUri = "http://sns.whalecloud.com/sina2/callback";
        }

        public SinaAuthorization()
        {
        }

        private void UpdateTokenInfo(SdkAuth2Res res)
        {
            if (TokenInfo == null)
                TokenInfo = new TokenInfo();

            TokenInfo.access_token = res.AccessToken;
            TokenInfo.Expires_In = int.Parse(res.ExpriesIn);
            TokenInfo.Refresh_Token = res.RefreshToken;
            TokenInfo.source = LoginType.Sina.Convert();
            StoreTokenInfo();
        }

        public override bool IsAuthorized
        {
            get
            {
                return _oauth.IsAuthorized;
            }
        }

        public override async Task Login(ZhiHuLoginInfo info, Action<RiBaoAuthoInfo> loginCallback)
        {
            if (IsAuthorized && TokenInfo != null)
            {
                var zhiHuAuthoData = await LoginZhiHu();
                loginCallback?.Invoke(zhiHuAuthoData);
                return;
            }
            try
            {
                _oauth.LoginCallback += async (isSuccess, err, response) =>
                {
                    if (isSuccess)
                    {
                        _oauth.IsAuthorized = true;
                        UpdateTokenInfo(response);
                        var zhiHuAuthoData = await LoginZhiHu();
                        loginCallback?.Invoke(zhiHuAuthoData);
                    }
                    else
                    {
                        loginCallback?.Invoke(null);
                    }
                };
                _oauth.BeginOAuth();
            }
            catch
            {
                loginCallback?.Invoke(null);
            }
        }

        public void LoginForShare(Action<bool> loginCallback)
        {
            if (IsAuthorized && TokenInfo != null)
            {
                loginCallback?.Invoke(true);
                return;
            }
            _oauth.LoginCallback += (isSuccess, err, response) =>
            {
                if (isSuccess)
                {
                    _oauth.IsAuthorized = true;
                    UpdateTokenInfo(response);
                    loginCallback?.Invoke(true);
                }
                else
                {
                    loginCallback?.Invoke(false);
                }
            };
            _oauth.BeginOAuth();
        }

        Task<RiBaoAuthoInfo> LoginZhiHu()
        {
            return DataRequester.LoginUsingWeibo(TokenInfo);
        }

        public override void Logout()
        {
            base.Logout();
            Storager.Remove("access_token");
            _oauth.IsAuthorized = false;
        }
    }
}
