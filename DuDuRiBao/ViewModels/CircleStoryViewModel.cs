using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public class CircleStoryViewModel : ViewModelBase
    {
        public string CircleId { get; set; }

        public CircleInfo Circle { get; set; }

        private string _currentDate;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        public CircleStoryViewModel()
        {
            
        }

        public async Task Refresh()
        {
            await RefreshCircleInfo();
            await RefreshStories(false);
        }

        private async Task RefreshCircleInfo()
        {
            if (string.IsNullOrEmpty(CircleId))
                return;

            Circle = await DataRequester.RequestCircleInfo(CircleId);
            Notify("Circle");
        }

        public async Task RefreshStories(bool isLoadingMore)
        {
            HotCircleStories hotCircleStories = null;

            if (isLoadingMore)
            {
                if (StoryDataList.Count > 0)
                {
                    hotCircleStories = await DataRequester.RequestNextStoriesForCircle(CircleId, StoryDataList.Last().Time.ToString());
                }
            }
            else
            {
                ResetStorys();
                hotCircleStories = await DataRequester.RequestLatestStoriesForCircle(CircleId);
                if (hotCircleStories != null && hotCircleStories.Stories != null && hotCircleStories.Stories.Count > 0)
                {
                    CurrentStoryId = hotCircleStories.Stories.First().Id.ToString();
                }
            }

            if (hotCircleStories == null || hotCircleStories.Stories == null || hotCircleStories.Stories.Count == 0)
                return;

            hotCircleStories.Stories.ForEach(story => story.AdjustForHotCircleStory(Circle));
            StoryDataList.AddRange(hotCircleStories.Stories);
        }

        protected void ResetStorys()
        {
            _currentDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            StoryDataList.Clear();
        }
    }
}
