using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using Brook.DuDuRiBao.ViewModels;
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
    public sealed partial class CircleStoryPage : PageBase
    {
        public CircleStoryViewModel VM { get { return GetVM<CircleStoryViewModel>(); } }

        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        private bool _isLoadComplete = false;

        public CircleStoryPage()
        {
            this.InitializeComponent();
            StoryListView.Refresh = RefreshMainList;
            StoryListView.LoadMore = LoadMoreStories;
            Loaded += CircleStoryPage_Loaded;
        }

        private void CircleStoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            StoryListView.SetRefresh(true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            VM.CircleId = e.Parameter.ToString();
            VM.RefreshCircleInfo();
        }

        private void StoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var story = e.ClickedItem as Story;
            if (story == null)
                return;

            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.DisplayStory, Content = story.Id.ToString() });
        }

        private async void RefreshMainList()
        {
            await VM.RefreshCircleInfo();
            StoryListView.SetRefresh(false);
            if (!Config.IsSinglePage)
            {
                //DisplayStory(ViewModelBase.CurrentStoryId);
            }
        }

        private async void LoadMoreStories()
        {
            if (_isLoadComplete)
            {
                StoryListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.StoryDataList.Count;
            //await VM.LoadMore();
            StoryListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.StoryDataList.Count;
        }
    }
}
