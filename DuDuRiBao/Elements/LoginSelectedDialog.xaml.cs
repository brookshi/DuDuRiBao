using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Utils;
using DuDuRiBao.Utils;
using LLQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class LoginSelectedDialog : DialogBase
    {
        public LoginSelectedDialog()
        {
            this.InitializeComponent();
        }

        private async void OnZhiHuLogin(object sender, RoutedEventArgs e)
        {
            Hide();
            ZhiHuLoginDialog dlg = new ZhiHuLoginDialog();
            await dlg.ShowAsync();
        }

        private void OnWeiBoLogin(object sender, RoutedEventArgs e)
        {
            Hide();
            LoadingIcon.Display();
            AuthorizationHelper.Login(LoginType.Sina, null, loginSuccess =>
            {
                if (loginSuccess)
                {
                    PopupMessage.DisplayMessageInRes("LoginSuccess");
                    var info = StorageUtil.StorageInfo.ZhiHuAuthoInfo;
                    if (info == null)
                        return;

                    LLQNotifier.Default.Notify(new LoginEvent() { IsLogin = true, UserPhotoUrl = info.avatar });
                }
                else
                {
                    PopupMessage.DisplayMessageInRes("LoginFailed");
                }
                LoadingIcon.Hide();
            });
        }
    }
}
