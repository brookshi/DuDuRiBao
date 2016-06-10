using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Pages;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class HotCirclesPage : PageBase
    {
        public HotCircleViewModel VM { get { return GetVM<HotCircleViewModel>(); } }

        public HotCirclesPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            LLQNotifier.Default.Register(this);
            Loaded += HotCirclesPage_Loaded;
        }

        private void HotCirclesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            VM.RefreshHotCircle();
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string data = e.Value;
            if (!string.IsNullOrEmpty(data) && data.StartsWith(Html.NotifyPrex))
            {
                data = data.ToLower();
                var id = data.Substring(data.LastIndexOf("/") + 1);
                NavigationManager.Instance.Navigate(Frame, typeof(CircleStoryPage), id);
            }
        }

        [SubscriberCallback(typeof(StoryEvent))]
        private void Subscriber(StoryEvent param)
        {
            switch (param.Type)
            {
                case StoryEventType.Night:
                    if (VM != null)
                        VM.RefreshHotCircle();
                    break;
            }
        }
    }
}
