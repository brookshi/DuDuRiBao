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
            //SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            LLQNotifier.Default.Register(this);
        }

        //private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    if (TimeLineFrame.CanGoBack && e.Handled == false)
        //    {
        //        e.Handled = true;
        //        TimeLineFrame.GoBack();
        //    }
        //    else if (Searcher.Visibility == Visibility.Visible)
        //    {
        //        HideSearcher();
        //        e.Handled = true;
        //    }
        //    NavigationManager.Instance.UpdateGoBackBtnVisibility();
        //}

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
                await StatusBar.GetForCurrentView().HideAsync();
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
                case StoryEventType.Menu:
                    ResetCategoryPanel();
                    break;
                case StoryEventType.Comment:
                    if (!Config.IsSinglePage)
                    {
                        StoryContentView.IsPaneOpen = !StoryContentView.IsPaneOpen;
                    }

                    if (Config.UIStatus == AppUIStatus.All)
                    {
                        StorageUtil.SetCommentPanelStatus(StoryContentView.IsPaneOpen);
                    }
                    break;
                case StoryEventType.Search:
                    ShowSearcher();
                    break;
                case StoryEventType.Night:
                    if (VM != null)
                        VM.RefreshExplore();
                    break;
                case StoryEventType.DisplayStory:
                    DisplayStory(param.Content);
                    break;
                case StoryEventType.Circle:
                    NavigationManager.Instance.Navigate(TimeLineFrame, typeof(CircleStoryPage), param.Content);
                    break;
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
                    UpdateCircle(int.Parse(circle.Id), circle.Name, circle, false);
                    HideSearcher();
                    break;
                case SearchType.Story:
                    var story = (SearchStory)param.SearchObj;
                    DisplayStory(story.Id.ToString());
                    break;
            }
        }

        private void RefershHotCircle_Click(object sender, RoutedEventArgs e)
        {
            VM.RefreshHotCircles();
        }

        private void ResetCategoryPanel()
        {
            MainView.IsPaneOpen = !MainView.IsPaneOpen;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            UpdateCircle(Misc.Default_Category_Id, (sender as Button).Content.ToString(), null, true);
        }

        private void HotArticle_Click(object sender, RoutedEventArgs e)
        {
            UpdateCircle(Misc.HotArtical_Category_Id, (sender as Button).Content.ToString(), null, true);
        }

        private void MyFav_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }
            UpdateCircle(Misc.Favorite_Category_Id, StringUtil.GetString("FavCategoryName"), null, true);
        }

        private void NightModeBtn_Toggled(object sender, RoutedEventArgs e)
        {
            StorageUtil.UpdateStorageInfo();
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            DisplayStory(Misc.Feedback_Story_Id.ToString());
            ResetCategoryPanel();
        }

        private void HotCircleListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var circle = e.ClickedItem as HotCircle;
            UpdateCircle(int.Parse(circle.Id), circle.Name, circle, true);
        }

        private void UpdateCircle(int categoryId, string circleName, CircleBase circle, bool needResetPanel)
        {
            VM.CurrentCategoryId = categoryId;
            VM.CategoryName = circleName;
            if (circle != null)
            {
                VM.CurrentCircle = circle;
            }
            //MainListView.SetRefresh(true);
            if (needResetPanel)
            {
                ResetCategoryPanel();
            }
        }
    }
}
