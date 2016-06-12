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
    public sealed partial class FavoritePage : PageBase
    {
        public bool IsDesktopDevice { get { return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }
        public FavoriteViewModel VM { get { return GetVM<FavoriteViewModel>(); } }
        private bool _isLoadComplete = false;

        public FavoritePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            FavoriteListView.Refresh = RefreshFavoriteList;
            FavoriteListView.LoadMore = LoadMoreStories;

            Loaded += FavoritePage_Loaded;
        }

        private void FavoritePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUsingCachedWhenNavigate())
            {
                return;
            }
            FavoriteListView.SetRefresh(true);
        }

        private void FavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var story = e.ClickedItem as Story;

            var storyId = story.Id.ToString();
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.DisplayStory, Content = storyId });
        }

        private async void RefreshFavoriteList()
        {
            await VM.Refresh();
            FavoriteListView.SetRefresh(false);
            if (!Config.IsSinglePage)
            {
                LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.DisplayStory, Content = ViewModelBase.CurrentStoryId });
            }
        }

        private async void LoadMoreStories()
        {
            if (_isLoadComplete)
            {
                FavoriteListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.StoryDataList.Count;
            await VM.LoadMore();
            FavoriteListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.StoryDataList.Count;
        }
    }
}
