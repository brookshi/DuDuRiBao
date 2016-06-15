using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class ZhiHuLoginDialog : DialogBase, INotifyPropertyChanged
    {
        public ZhiHuLoginDialog()
        {
            this.InitializeComponent();
        }

        protected override void Load()
        {
            IsLoginBtnEnabled = false;
            GetCaptchaImage();
        }

        private async void GetCaptchaImage()
        {
            var captcha = await DataRequester.GetCaptchaImage();
            if (captcha == null)
            {
                PopupMessage.DisplayMessage(StringUtil.GetString("GetCaptchaFailed"));
                CaptchaImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/CaptchaFailed.png"));
                return;
            }

            if (!string.IsNullOrEmpty(captcha.Img_Base64))
            {
                NeedCaptcha = true;
                ResUtil.SetBase64ToImage((BitmapSource)CaptchaImage.Source, captcha.Img_Base64);
            }
            else
            {
                NeedCaptcha = false;
            }
        }

        private bool _needCaptcha = false;
        public bool NeedCaptcha
        {
            get
            {
                return _needCaptcha;
            }
            set
            {
                if(value != _needCaptcha)
                {
                    _needCaptcha = value;
                    Notify("NeedCaptcha");
                }
            }
        }

        private bool _isLoginBtnEnabled = true;
        public bool IsLoginBtnEnabled
        {
            get { return _isLoginBtnEnabled; }
            set
            {
                if(value != _isLoginBtnEnabled)
                {
                    _isLoginBtnEnabled = value;
                    Notify("IsLoginBtnEnabled");
                }
            }
        }

        private void ParamChanged(object sender, RoutedEventArgs e)
        {
            IsLoginBtnEnabled = !string.IsNullOrEmpty(PasswordTxt.Password.Trim()) &&
                                (!NeedCaptcha || !string.IsNullOrEmpty(CaptchaTxt.Text.Trim())) &&
                                (StringUtil.CheckEmail(UserNameTxt.Text.Trim()) ||
                                StringUtil.CheckPhoneNum(UserNameTxt.Text.Trim()));

        }

        private void RefreshCaptchaImage(object sender, RoutedEventArgs e)
        {
            GetCaptchaImage();
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            var userName = UserNameTxt.Text.Trim();
            userName = StringUtil.CheckPhoneNum(userName) ? "+86" + userName : userName;
            var password = PasswordTxt.Password.Trim();
            var captcha = CaptchaTxt.Text.Trim();

            LoadingIcon.Display();
            AuthorizationHelper.Login(LoginType.ZhiHu, new ZhiHuLoginInfo() { Captcha = NeedCaptcha ? captcha : null, UserName = userName, Password = password }, loginSuccess =>
            {
                if (loginSuccess)
                {
                    PopupMessage.DisplayMessageInRes("LoginSuccess");
                    var info = StorageInfo.Instance.ZhiHuAuthoInfo;
                    if (info == null)
                        return;

                    LLQNotifier.Default.Notify(new LoginEvent() { IsLogin = true, UserPhotoUrl = info.avatar });
                    Hide();
                }
                else
                {
                    GetCaptchaImage();
                }
                LoadingIcon.Hide();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
