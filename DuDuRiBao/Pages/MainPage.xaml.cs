using LLQ;
using Brook.DuDuRiBao.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Common;
using Windows.UI.Core;
using Brook.DuDuRiBao.Utils;
using Windows.UI.ViewManagement;
using Brook.DuDuRiBao.ViewModels;
using Brook.DuDuRiBao.Authorization;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using System;
using LLM;
using DuDuRiBao.Utils;
using Windows.UI.Xaml.Media;

namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class MainPage : PageBase
    {
        public MainViewModel VM { get { return GetVM<MainViewModel>(); } }

        public bool IsCommentPanelOpen { get { return true; } }

        public MainPage()
        {
            this.InitializeComponent();
            Initalize();
            NavigationCacheMode = NavigationCacheMode.Required;

            Loaded += MainPage_Loaded;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            LLQNotifier.Default.Register(this);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Searcher.Visibility = Visibility.Collapsed;
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            await InitUI();

            var isLogin = await AuthorizationHelper.AutoLogin();
            if (isLogin)
                UpdateLoginInfo();

            TimeLineFrame.Navigate(typeof(TimeLinePage));
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Searcher.Visibility == Visibility.Visible)
            {
                HideSearcher();
                e.Handled = true;
            }
        }

        private void UpdateLoginInfo()
        {
            var info = StorageInfo.Instance.ZhiHuAuthoInfo;
            if (info == null)
                return;

            LLQNotifier.Default.Notify(new LoginEvent() { IsLogin = true, UserPhotoUrl = info.avatar });
        }

        private static async Task InitUI()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            var isStatusBarPresent = ApiInformation.IsTypePresent(typeof(StatusBar).ToString());
            if (isStatusBarPresent)
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Windows.UI.Colors.White;
                    statusBar.BackgroundColor = StorageInfo.Instance.AppTheme == ElementTheme.Light ? ColorUtil.GetColorFromHexString("#1976D2") : ColorUtil.GetColorFromHexString("#0e0e0f");
                    await statusBar.ShowAsync();
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Config.IsPageSwitched(e.PreviousSize, e.NewSize) && !string.IsNullOrEmpty(ViewModelBase.CurrentStoryId))
            {
                MainContentFrame.Navigate(typeof(MainContentPage));
                CommentFrame.Navigate(typeof(CommentPage));
            }
        }

        private void DisplayStory(string storyId)
        {
            ViewModelBase.CurrentStoryId = storyId;
            if (Config.UIStatus == AppUIStatus.All || Config.UIStatus == AppUIStatus.ListAndContent)
            {
                MainContentFrame.Navigate(typeof(MainContentPage), storyId);
                CommentFrame.Navigate(typeof(CommentPage), storyId);
            }
            else
            {
                Frame rootFrame = App.GetWindowFrame();
                if (rootFrame == null)
                    return;

                NavigationManager.Instance.Navigate(rootFrame, typeof(MainContentPage), storyId);
            }
        }

        [SubscriberCallback(typeof(StoryEvent))]
        private void Subscriber(StoryEvent param)
        {
            switch (param.Type)
            {
                case StoryEventType.Comment:
                    if (!Config.IsSinglePage)
                    {
                        StoryContentView.IsPaneOpen = !StoryContentView.IsPaneOpen;
                    }

                    if (Config.UIStatus == AppUIStatus.All)
                    {
                        Storager.SetCommentPanelStatus(StoryContentView.IsPaneOpen);
                    }
                    break;
                case StoryEventType.Search:
                    ShowSearcher();
                    break;
                case StoryEventType.DisplayStory:
                    DisplayStory(param.Content);
                    break;
                case StoryEventType.Circle:
                    Navigate(typeof(CircleStoryPage), param.Content);
                    break;
                case StoryEventType.HotCircle:
                    Navigate(typeof(HotCirclesPage), null);
                    break;
                case StoryEventType.HotStory:
                    Navigate(typeof(HotStoriesPage), null);
                    break;
                case StoryEventType.FavPage:
                    Navigate(typeof(FavoritePage), null);
                    break;
                case StoryEventType.Setting:
                    if (TimeLineFrame.Content is SettingPage)
                        return;
                    Navigate(typeof(SettingPage), null);
                    break;
                case StoryEventType.Night:
                    InitUI();
                    break;
            }
        }

        void Navigate(Type type, object content)
        {
            if (Config.UIStatus == AppUIStatus.All || Config.UIStatus == AppUIStatus.ListAndContent)
            {
                NavigationManager.Instance.Navigate(TimeLineFrame, type, content);
            }
            else
            {
                NavigationManager.Instance.Navigate(Frame, type, content);
            }
        }

        void ShowSearcher()
        {
            Searcher.Visibility = Visibility.Visible;
            Animator.Use(AnimationType.FadeInDown).SetDuration(TimeSpan.FromMilliseconds(300)).PlayOn(Searcher, ()=> Searcher.FocusText());
        }

        void HideSearcher()
        {
            Animator.Use(AnimationType.FadeOutUp).SetDuration(TimeSpan.FromMilliseconds(300)).PlayOn(Searcher, () => Searcher.Visibility = Visibility.Collapsed);
        }

        [SubscriberCallback(typeof(SearchEvent))]
        private void SearchSubscriber(SearchEvent param)
        {
            switch (param.Type)
            {
                case SearchType.Circle:
                    var circle = (CircleBase)param.SearchObj;
                    circle.Name = circle.Name.Replace("<em>", "").Replace("</em>", "");
                    Navigate(typeof(CircleStoryPage), circle.Id);
                    HideSearcher();
                    break;
                case SearchType.Story:
                    var story = (SearchStory)param.SearchObj;
                    DisplayStory(story.Id.ToString());
                    break;
            }
        }
    }
}
