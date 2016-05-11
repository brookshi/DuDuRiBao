using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class MainViewModel
    {
        public int? FavoritesLastTime { get; set; } = null;

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
                ResetStorys();
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
    }
}
