#region License
//   Copyright 2015 Brook Shi
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private string _currentDate;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        private List<bool> _indicators = new List<bool>();

        public List<bool> Indicators { get { return _indicators; } set { if (value != _indicators) { _indicators = value; Notify("Indicators"); } } }

        public override void Init()
        {
            RefreshHotCircles();
        }

        public async Task Refresh()
        {
            ResetStorys();
            await RequestMainList(false);

            if (StoryDataList.Count < Misc.Page_Count)
            {
                await LoadMore();
            }
        }

        public async Task LoadMore()
        {
            await RequestMainList(true);
        }

        protected void ResetStorys()
        {
            _currentDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            StoryDataList.Clear();
            Indicators.Clear();
        }

        private async Task RequestMainList(bool isLoadingMore)
        {
            if(CurrentCategoryId == Misc.Default_Category_Id)
            {
                await RequestDefaultCategoryData(isLoadingMore);
            }
            else if (CurrentCategoryId == Misc.Favorite_Category_Id)
            {
                await RequestFavorites(isLoadingMore);
            }
            else if (CurrentCategoryId == Misc.HotArtical_Category_Id)
            {
                await RequestHotArticles(isLoadingMore);
            }
            else
            {
                await RequestMinorCategoryData(isLoadingMore);
            }
        }

        private async Task RequestDefaultCategoryData(bool isLoadingMore)
        {
            TimeLine timeLine = null;

            if (isLoadingMore)
            {
                if (StoryDataList.Count > 0)
                {
                    timeLine = await DataRequester.RequestNextTimeLine(StoryDataList.Last().Time.ToString());
                }
            }
            else
            {
                ResetStorys();
                timeLine = await DataRequester.RequestLatestTimeLine();
                if (timeLine != null)
                {
                    CurrentStoryId = timeLine.Items.First().Id.ToString();
                }
            }

            if (timeLine == null)
                return;
            timeLine.Adjust();
            StoryDataList.AddRange(timeLine.Items);
        }

        private async Task RequestMinorCategoryData(bool isLoadingMore)
        {
            HotCircleStories hotCircleStories = null;

            if (isLoadingMore)
            {
                if (StoryDataList.Count > 0)
                {
                    hotCircleStories = await DataRequester.RequestNextStoriesForCircle(CurrentCategoryId.ToString(), StoryDataList.Last().Time.ToString());
                }
            }
            else
            {
                ResetStorys();
                hotCircleStories = await DataRequester.RequestLatestStoriesForCircle(CurrentCategoryId.ToString());
                if (hotCircleStories != null && hotCircleStories.Stories != null && hotCircleStories.Stories.Count > 0)
                {
                    CurrentStoryId = hotCircleStories.Stories.First().Id.ToString();
                }
            }

            if (hotCircleStories == null && hotCircleStories.Stories != null)
                return;

            hotCircleStories.Stories.ForEach(story => story.AdjustPosterForHotCircleStory(CategoryName));
            StoryDataList.AddRange(hotCircleStories.Stories);
        }

        private void UpdateIndicators(int count)
        {
            var indicators = new List<bool>();
            for(int i=0;i< count; i++)
            {
                indicators.Add(true);
            }
            Indicators = indicators;
        }
    }
}
