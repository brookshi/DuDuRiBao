using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using Brook.DuDuRiBao.ViewModels;
using DuDuRiBao.Utils;
using LLQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class CreateCirclePage : PageBase
    {
        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        public Visibility ToolBarVisibility { get { return Config.UIStatus == AppUIStatus.List ? Visibility.Visible : Visibility.Collapsed; } }

        public CreateCirclePage()
        {
            this.InitializeComponent();
        }

        private async void CreateCircle(object sender, RoutedEventArgs arg)
        {
            var title = CircleTitle.Text;
            var desc = CircleDesc.Text;
            if (string.IsNullOrEmpty(title))
            {
                PopupMessage.DisplayMessageInRes("CreateCircleError");
                return;
            }

            var rst = await DataRequester.CreateCircle(title, desc);
            if(rst.Id < 1)
            {
                PopupMessage.DisplayMessage(rst.Error_Msg);
                return;
            }

            PopupMessage.DisplayMessageInRes("CreateCircleSuccess");
            NavigationManager.Instance.GoBack();
        }
    }
}
