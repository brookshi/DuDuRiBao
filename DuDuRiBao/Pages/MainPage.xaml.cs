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

namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class MainPage : PageBase
    {
        public MainViewModel VM { get { return GetVM<MainViewModel>(); } }

        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        public bool IsCommentPanelOpen { get { return true; } }

        private bool _isLoadComplete = false;

        public MainPage()
        {
            this.InitializeComponent();
            Initalize();
            NavigationCacheMode = NavigationCacheMode.Required;

            MainListView.Refresh = RefreshMainList;
            MainListView.LoadMore = LoadMoreStories;

            Loaded += MainPage_Loaded;

            LLQNotifier.Default.Register(this);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            await InitUI();

            var isLogin = await AuthorizationHelper.AutoLogin();
            if (isLogin)
                UpdateLoginInfo();

            MainListView.SetRefresh(true);
        }

        private void UpdateLoginInfo()
        {
            var info = StorageUtil.StorageInfo.ZhiHuAuthoInfo;
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

        private async void RefreshMainList()
        {
            await VM.Refresh();
            MainListView.SetRefresh(false);
            if (!Config.IsSinglePage)
            {
                DisplayStory(ViewModelBase.CurrentStoryId);
            }
        }

        private async void LoadMoreStories()
        {
            if (_isLoadComplete)
            {
                MainListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.StoryDataList.Count;
            await VM.LoadMore();
            MainListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.StoryDataList.Count;
        }

        private void MainListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var story = e.ClickedItem as Story;

            var storyId = story.Id.ToString();
            DisplayStory(storyId);
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

                rootFrame.Navigate(typeof(MainContentPage), storyId);
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
            VM.CurrentCategoryId = Misc.Default_Category_Id;
            VM.CategoryName = (sender as Button).Content.ToString();
            MainListView.SetRefresh(true);
            ResetCategoryPanel();
        }

        private void HotArticle_Click(object sender, RoutedEventArgs e)
        {
            VM.CurrentCategoryId = Misc.HotArtical_Category_Id;
            VM.CategoryName = (sender as Button).Content.ToString();
            MainListView.SetRefresh(true);
            ResetCategoryPanel();
        }

        private void MyFav_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            VM.CurrentCategoryId = Misc.Favorite_Category_Id;
            VM.CategoryName = StringUtil.GetString("FavCategoryName");
            MainListView.SetRefresh(true);
            ResetCategoryPanel();
        }

        private void HotCircleListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var circel = e.ClickedItem as HotCircle;
            VM.CurrentCategoryId = int.Parse(circel.Id);
            VM.CategoryName = circel.Name;
            VM.CurrentCircle = circel;
            MainListView.SetRefresh(true);
            ResetCategoryPanel();
        }
    }
}
