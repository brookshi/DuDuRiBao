using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Utils;
using Brook.DuDuRiBao.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using LLQ;
using Brook.DuDuRiBao.Events;

namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class MainContentPage : Page
    {
        public MainContentViewModel VM { get { return DataContext as MainContentViewModel; } }

        public MainContentPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            LLQNotifier.Default.Register(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WebViewUtil.AddWebViewWithBinding(MainContent, VM, "MainHtmlContent");

            if (Config.UIStatus == AppUIStatus.List)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            CurrentUIStatus = Config.UIStatus;
            VM.RequestStoryData();
            LLQNotifier.Default.Notify(new OpenNewStoryEvent());
        }

        public Visibility ToolBarVisibility { get { return Config.IsSinglePageStatus(CurrentUIStatus) ? Visibility.Visible : Visibility.Collapsed; } }

        public AppUIStatus CurrentUIStatus { get; set; }

        [SubscriberCallback(typeof(StoryEvent))]
        private void Subscriber(StoryEvent param)
        {
            switch (param.Type)
            {
                case StoryEventType.Comment:
                    if (Config.IsSinglePageStatus(CurrentUIStatus))
                    {
                        Frame rootFrame = App.GetWindowFrame();
                        if (rootFrame == null)
                            return;

                        rootFrame.Navigate(typeof(CommentPage));
                    }
                    break;
            }
        }
    }
}
