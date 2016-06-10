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
    public partial class TimeLineViewModel : ViewModelBase
    {
        private string _currentDate;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        private string _explore;
        public string Explore
        {
            get { return _explore; }
            set
            {
                if (value != _explore)
                {
                    _explore = value;
                    Notify("Explore");
                }
            }
        }

        public override void Init()
        {
            RefreshExplore();
        }

        public async Task Refresh()
        {
            ResetStories();
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

        public async void RefreshExplore()
        {
            Explore = await DataRequester.RequestExplore();
        }

        protected void ResetStories()
        {
            _currentDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            StoryDataList.Clear();
        }

        private async Task RequestMainList(bool isLoadingMore)
        {
            await RequestStories(isLoadingMore);
        }

        private async Task RequestStories(bool isLoadingMore)
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
                ResetStories();
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
    }
}
