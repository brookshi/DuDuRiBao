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
    public sealed partial class CircleStoryPage : PageBase
    {
        public CircleStoryViewModel VM { get { return GetVM<CircleStoryViewModel>(); } }

        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        private bool _isLoadComplete = false;

        public CircleStoryPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            StoryListView.Refresh = RefreshMainList;
            StoryListView.LoadMore = LoadMoreStories;
            StoryListView.FloatButtonAction = VM.JoinCircle;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationManager.Instance.UpdateGoBackBtnVisibility();
            VM.CircleId = e.Parameter.ToString();
            StoryListView.SetRefresh(true);
        }

        private void StoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var story = e.ClickedItem as Story;
            if (story == null)
                return;

            ViewModelBase.CurrentStoryId = story.Id.ToString();
            DisplayStory();
        }

        private async void RefreshMainList()
        {
            await VM.Refresh();
            StoryListView.SetRefresh(false);
            if (!Config.IsSinglePage)
            {
                DisplayStory();
            }
        }

        private void DisplayStory()
        {
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.DisplayStory, Content = ViewModelBase.CurrentStoryId.ToString() });
        }

        private async void LoadMoreStories()
        {
            if (_isLoadComplete)
            {
                StoryListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.StoryDataList.Count;
            await VM.RefreshStories(true);
            StoryListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.StoryDataList.Count;
        }
    }
}
