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

        private string _userPhotoUrl = "ms-appx:///Assets/Login.png";
        public string UserPhotoUrl
        {
            get { return _userPhotoUrl; }
            set
            {
                if(value != _userPhotoUrl)
                {
                    _userPhotoUrl = value;
                    Notify("UserPhotoUrl");
                }
            }
        }

        private string _userName = StringUtil.GetString("PleaseLogin");
        public string UserName
        {
            get { return _userName; }
            set
            {
                if(value != _userName)
                {
                    _userName = value;
                    Notify("UserName");
                }
            }
        }

        public override void Init()
        {
            //InitCategories();
           // RequestMainList(false);
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
            //if(CurrentCategoryId == Misc.Default_Category_Id)
            {
                await RequestDefaultCategoryData(isLoadingMore);
            }
            //else if(CurrentCategoryId == Misc.Favorite_Category_Id)
            //{
            //    await RequestFavorites(isLoadingMore);
            //}
            //else
            //{
            //    await RequestMinorCategoryData(isLoadingMore);
            //}
        }

        private async Task RequestDefaultCategoryData(bool isLoadingMore)
        {
            TimeLine timeLine = null;

            //if (isLoadingMore)
            //{
            //    timeLine = await DataRequester.RequestStories(_currentDate);
            //}
            //else
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

           // _currentDate = timeLine;

            //StoryDataList.Add(new Story() { title = StringUtil.GetStoryGroupName(_currentDate), type = Misc.Group_Name_Type });
            StoryDataList.AddRange(timeLine.Items);
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

        public void LoginSuccess()
        {
            var info = StorageUtil.StorageInfo.ZhiHuAuthoInfo;
            if (info == null)
                return;

            UserPhotoUrl = info.avatar;
            UserName = info.name;
        }
    }
}
