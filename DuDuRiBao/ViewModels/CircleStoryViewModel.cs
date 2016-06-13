using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Brook.DuDuRiBao.ViewModels
{
    public class CircleStoryViewModel : ViewModelBase
    {
        private static IconElement _addIcon = new SymbolIcon(Symbol.Add);
        private static IconElement _checkIcon = new SymbolIcon(Symbol.Accept);

        public string CircleId { get; set; }

        public CircleInfo Circle { get; set; }

        private string _currentDate;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        private string _title = StringUtil.GetString("DefaultTitle");
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Notify("Title");
            }
        }

        private IconElement _joinCircleButtonIcon = _addIcon;
        public IconElement JoinCircleButtonIcon
        {
            get { return _joinCircleButtonIcon; }
            set
            {
                _joinCircleButtonIcon = value;
                Notify("JoinCircleButtonIcon");
            }
        }

        public CircleStoryViewModel()
        {
            
        }

        public async Task Refresh()
        {
            await RefreshCircleInfo();
            if (Circle == null)
                return;

            await RefreshStories(false);
        }

        private async Task RefreshCircleInfo()
        {
            if (string.IsNullOrEmpty(CircleId))
                return;

            Circle = await DataRequester.RequestCircleInfo(CircleId);
            if (Circle == null)
                return;

            Title = Circle.Name;
            if (string.IsNullOrEmpty(Circle.Image))
                Circle.Image = Circle.Thumbnail;
            Circle.Adjust();

            JoinCircleButtonIcon = Circle.Status == 1 ? _checkIcon : _addIcon;
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

        public async void JoinCircle()
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            if(Misc.ZhiHuCircleId.ToString() == CircleId && JoinCircleButtonIcon == _checkIcon)
            {
                PopupMessage.DisplayMessageInRes("CannotQuitZhiHuCircle");
                return;
            }

            if (JoinCircleButtonIcon == _addIcon)
            {
                JoinCircleButtonIcon = _checkIcon;
                await DataRequester.JoinCircle(CircleId);
                PopupMessage.DisplayMessageInRes("JoinCircleSuccess");
            }
            else
            {
                JoinCircleButtonIcon = _addIcon;
                await DataRequester.QuitCircle(CircleId);
                PopupMessage.DisplayMessageInRes("QuitCircleSuccess");
            }
        }
    }
}
