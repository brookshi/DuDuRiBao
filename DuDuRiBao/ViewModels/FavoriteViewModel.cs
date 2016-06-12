using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public class FavoriteViewModel : ViewModelBase
    {
        public int? FavoritesLastTime { get; set; } = null;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        public async Task Refresh()
        {
            await RequestFavorites(false);
        }

        public async Task LoadMore()
        {
            await RequestFavorites(true);
        }

        private async Task RequestFavorites(bool isLoadingMore)
        {
            Favorites favData = null;

            if (isLoadingMore)
            {
                if (!FavoritesLastTime.HasValue)
                    return;

                favData = await DataRequester.RequestFavorites(FavoritesLastTime.Value.ToString());
            }
            else
            {
                ResetStories();
                favData = await DataRequester.RequestLatestFavorites();
                if (favData != null && favData.stories != null && favData.stories.Count > 0)
                {
                    CurrentStoryId = favData.stories.First().Id.ToString();
                }
            }

            if (favData == null)
                return;

            FavoritesLastTime = favData.last_time;

            StoryDataList.AddRange(favData.stories);
        }

        protected void ResetStories()
        {
            StoryDataList.Clear();
        }
    }
}
