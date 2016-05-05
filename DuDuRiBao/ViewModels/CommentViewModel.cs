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
        private readonly ObservableCollectionExtended<Comment> _commentList = new ObservableCollectionExtended<Comment>();

        public ObservableCollectionExtended<Comment> CommentList { get { return _commentList; } }

        public string LastCommentId
        {
            get { return CommentList.LastOrDefault()?.Id.ToString() ?? null; }
        }

        public string Title
        {
            get { return StringUtil.GetString("CommentTitle"); }
        }

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

        public void ResetComments()
        {
            CommentList.Clear();
        }

        public async Task RequestComments(bool isLoadingMore)
        {
            if (!isLoadingMore)
            {
                ResetComments();
            }

            var comments = await DataRequester.RequestComments(CurrentStoryId, LastCommentId);
            if (comments == null || comments.Comments == null || comments.Comments.Count == 0)
                return;

            CommentList.AddRange(comments.Comments);

            if (CommentList.Count < Misc.Page_Count)
            {
                await RequestComments(true);
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
                    var comment = CommentList.SingleOrDefault(o => o.Id == param.Comment.Id);
                    if(comment != null)
                    {
                        CommentList.Remove(comment);
                        return;
                    }

                    comment = CommentList.SingleOrDefault(o => o.Id == param.Comment.Id);
                    if (comment != null)
                    {
                        CommentList.Remove(comment);
                        return;
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
