using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
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
    public sealed partial class HotStoriesPage : PageBase
    {
        public HotStoriesViewModel VM { get { return GetVM<HotStoriesViewModel>(); } }

        public HotStoriesPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            LLQNotifier.Default.Register(this);
            Loaded += HotStoriesPage_Loaded;
        }

        private void HotStoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            VM.RefreshHotStories();
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string data = e.Value;
            if (!string.IsNullOrEmpty(data) && data.StartsWith(Html.NotifyPrex))
            {
                data = data.ToLower();
                var id = data.Substring(data.LastIndexOf("/") + 1);
                LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.DisplayStory, Content = id });
            }
        }

        [SubscriberCallback(typeof(StoryEvent))]
        private void Subscriber(StoryEvent param)
        {
            switch (param.Type)
            {
                case StoryEventType.Night:
                    if (VM != null)
                        VM.RefreshHotStories();
                    break;
            }
        }
    }
}
