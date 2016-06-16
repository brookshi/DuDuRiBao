using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public class CommentViewModel : ViewModelBase
    {
        private CommentType _currCommentType = CommentType.Recommend;

        private readonly ObservableCollectionExtended<GroupComments> _commentList = new ObservableCollectionExtended<GroupComments>();

        public ObservableCollectionExtended<GroupComments> CommentList { get { return _commentList; } }

        public int TotalCount { get { return _commentList.Count > 1 ? _commentList.First().Count + _commentList.Last().Count : 0; } }

        public string LastCommentId
        {
            get { return CommentList.Count > 0 ? CommentList.Last().LastOrDefault()?.Id.ToString() ?? null : null; }
        }

        public string Title
        {
            get { return StringUtil.GetString("CommentTitle"); }
        }

        public bool IsRefreshing { get; set; }

        private bool _isReplingTo = false;
        public bool IsReplingTo
        {
            get { return _isReplingTo; }
            set
            {
                if(value != _isReplingTo)
                {
                    _isReplingTo = value;
                    Notify("IsReplingTo");
                }
            }
        }

        private string _replyTip;
        public string ReplyTip
        {
            get { return _replyTip; }
            set
            {
                if(value != _replyTip)
                {
                    _replyTip = value;
                    Notify("ReplyTip");
                }
            }
        }

        private string _postCommentContent;
        public string PostCommentContent
        {
            get { return _postCommentContent; }
            set
            {
                if (value != _postCommentContent)
                {
                    _postCommentContent = value;
                    Notify("PostCommentContent");
                }
            }
        }

        public int? ReplyCommentId { get; set; } = null;

        public CommentViewModel()
        {
            LLQNotifier.Default.Register(this);
        }

        static CommentViewModel()
        {
            LLQNotifier.Default.Register(CommentExclusiveSubscriber.Instance);
        }

        public async Task RequestComments(bool isLoadingMore)
        {
            if (!isLoadingMore)
            {
                ResetComments();
                InitCommentInfo();
            }

            if (_currCommentType == CommentType.Recommend)
            {
                await RequestRecommendComments();
            }
            else if (_currCommentType == CommentType.Normal)
            {
                await RequestNormalComments();
            }
        }

        private void InitCommentInfo()
        {
            if (CommentList.Count == 0)
            {
                CommentList.Add(new GroupComments() { GroupName = StringUtil.GetCommentGroupName(CommentType.Recommend, CurrentStoryExtraInfo.Count.Post_Reasons.ToString()) });
                CommentList.Add(new GroupComments() { GroupName = StringUtil.GetCommentGroupName(CommentType.Normal, CurrentStoryExtraInfo.Count.Normal_Comments.ToString()) });
            }
        }

        public void ResetComments()
        {
            CommentList.Clear();
            _currCommentType = CommentType.Recommend;
        }

        public int CurrentCommentCount { get { return CommentList.Count > 1 ? CommentList[0].Count + CommentList[1].Count : 0; } }

        private async Task RequestRecommendComments()
        {
            var recommendComments = await DataRequester.RequestRecommendComments(CurrentStoryId, LastCommentId);
            if (recommendComments == null)
                return;

            CommentList.First().AddRange(recommendComments.Comments);

            if (recommendComments == null || recommendComments.Comments == null || recommendComments.Comments.Count < Misc.Page_Count)
            {
                await RequestNormalComments();
            }
        }

        private async Task RequestNormalComments()
        {
            if (_currCommentType == CommentType.Recommend)
            {
                _currCommentType = CommentType.Normal;
            }
            var normalComments = await DataRequester.RequestNormalComments(CurrentStoryId, _currCommentType == CommentType.Recommend ? null : LastCommentId);
            if (normalComments != null)
            {
                CommentList.Last().AddRange(normalComments.Comments);
            }
        }

        public async Task SendComment()
        {
            await DataRequester.SendComment(CurrentStoryId, PostCommentContent, ReplyCommentId);
        }

        public void CancelReply()
        {
            IsReplingTo = false;
        }

        [SubscriberCallback(typeof(StoryExtraEvent))]
        private void StoryExtraSubscriber(StoryExtraEvent param)
        {
            if (CommentList.Count > 1)
            {
                CommentList[0].GroupName = StringUtil.GetCommentGroupName(CommentType.Recommend, CurrentStoryExtraInfo.Count.Post_Reasons.ToString());
                CommentList[1].GroupName = StringUtil.GetCommentGroupName(CommentType.Normal, CurrentStoryExtraInfo.Count.Normal_Comments.ToString());
            }
        }

        [SubscriberCallback(typeof(CommentEvent))]
        private void CommentSubscriber(CommentEvent param)
        {
            switch(param.Type)
            {
                case CommentEventType.Reply:
                    ReplyCommentId = param.Comment.Id;
                    IsReplingTo = true;
                    ReplyTip = string.Format(StringUtil.GetString("ReplyTip"), param.Comment.User.Name);
                    break;
                case CommentEventType.Delete:
                    if (CommentList.Count > 1)
                    {
                        var comment = CommentList[0].SingleOrDefault(o => o.Id == param.Comment.Id);
                        if (comment != null)
                        {
                            CommentList[0].Remove(comment);
                            return;
                        }

                        comment = CommentList[1].SingleOrDefault(o => o.Id == param.Comment.Id);
                        if (comment != null)
                        {
                            CommentList[1].Remove(comment);
                            return;
                        }
                    }
                    break;
            }
        }

        internal class CommentExclusiveSubscriber
        {
            internal static CommentExclusiveSubscriber Instance = new CommentExclusiveSubscriber();

            [SubscriberCallback(typeof(CommentEvent))]
            private void Subscriber(CommentEvent param)
            {
                var commentId = param.Comment.Id.ToString();
                switch (param.Type)
                {
                    case CommentEventType.Delete:
                        DataRequester.DeleteComment(commentId);
                        break;
                    case CommentEventType.Like:
                        if (param.IsLike)
                        {
                            DataRequester.LikeComment(commentId);
                        }
                        else
                        {
                            DataRequester.UnlikeComment(commentId);
                        }
                        break;
                }
            }
        }
    }
}
