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
    public sealed partial class PostToCirclePage : PageBase
    {
        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        public Visibility ToolBarVisibility { get { return Config.UIStatus == AppUIStatus.List ? Visibility.Visible : Visibility.Collapsed; } }

        public OwnCircleInfo CurrentCircle { get; set; }

        private bool _needCreateCircle = false;

        private OwnCircles OwnCircles;

        public PostToCirclePage()
        {
            this.InitializeComponent();
            Loaded += PostToCirclePage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _needCreateCircle = e.NavigationMode != NavigationMode.Back;
        }

        private async void PostToCirclePage_Loaded(object sender, RoutedEventArgs arg)
        {
            LoadingIcon.Display();
            OwnCircles = await DataRequester.GetOwnCircles();
            if(OwnCircles.Circles != null && OwnCircles.Circles.Count > 0)
            {
                CurrentCircle = OwnCircles.Circles[0];
                CurrentCircleBtn.Content = CurrentCircle.Name;
                OwnCircles.Circles.ForEach(circle => {
                    var menuItem = new MenuFlyoutItem() { Text = circle.Name, Tag = circle };
                    menuItem.Click += (s, e) => { CurrentCircle = ((OwnCircleInfo)((MenuFlyoutItem)s).Tag); CurrentCircleBtn.Content = CurrentCircle.Name; };
                    CircleMenu.Items.Add(menuItem);
                });
            }
            else if(_needCreateCircle)
            {
                LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.CreateCircle });
                PopupMessage.DisplayMessageInRes("NoCircle");
            }
            LoadingIcon.Hide();
        }

        private async void PostStory(object sender, RoutedEventArgs arg)
        {
            var title = StoryTitle.Text;
            var url = StoryUrl.Text;
            var reason = StoryReason.Text;
            if (CurrentCircle == null)
            {
                PopupMessage.DisplayMessageInRes("PostToCircleNeedCircle");
                return;
            }

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title))
            {
                PopupMessage.DisplayMessageInRes("PostToCircleError");
                return;
            }

           await DataRequester.PostToCircle(url, title, reason, new List<int>() { CurrentCircle.Id });
           
            PopupMessage.DisplayMessageInRes("PostToCircleSuccess");
            NavigationManager.Instance.GoBack();
        }
    }
}
