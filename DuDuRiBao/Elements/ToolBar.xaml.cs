using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Utils;
using LLM;
using LLQ;
using System.ComponentModel;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using XP;
using DuDuRiBao.Utils;

namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class ToolBar : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ToolBarHost Host { get; set; }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ToolBar), new PropertyMetadata(""));

        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(ToolBar), new PropertyMetadata(""));

        private string _userPhotoUrl = "ms-appx:///Assets/Login.png";
        public string UserPhotoUrl
        {
            get { return _userPhotoUrl; }
            set
            {
                if (value != _userPhotoUrl && !string.IsNullOrEmpty(value))
                {
                    _userPhotoUrl = value;
                    Notify("UserPhotoUrl");
                }
            }
        }

        private const string _shareUrlFormat = "https://www.zhihu.com/qrcode?url={0}";
        private string _shareUrl = "https://www.zhihu.com/qrcode?url=123";
        public string ShareUrl
        {
            get { return _shareUrl; }
            set
            {
                var url = string.Format(_shareUrlFormat, value);
                if (url != _shareUrl)
                {
                    _shareUrl = url;
                    Notify("ShareUrl");
                }
            }
        }

        private string _commentCount = "0";
        public string CommentCount
        {
            get { return _commentCount; }
            set
            {
                if (value != _commentCount)
                {
                    _commentCount = value;
                    Notify("CommentCount");
                }
            }
        }

        private string _likeCount = "0";
        public string LikeCount
        {
            get { return _likeCount; }
            set
            {
                if (value != _likeCount)
                {
                    _likeCount = value;
                    Notify("LikeCount");
                }
            }
        }

        private bool? _isLikeButtonChecked = false;
        public bool? IsLikeButtonChecked
        {
            get { return _isLikeButtonChecked; }
            set
            {
                _isLikeButtonChecked = value;
                Notify("IsLikeButtonChecked");
            }
        }

        private bool? _isFavoriteButtonChecked = false;
        public bool? IsFavoriteButtonChecked
        {
            get { return _isFavoriteButtonChecked; }
            set
            {
                _isFavoriteButtonChecked = value;
                Notify("IsFavoriteButtonChecked");
            }
        }

        public ToolBar()
        {
            this.InitializeComponent();
            WeiXinItem.DataContext = this;
            Loaded += (s, e) =>
            {
                if (this.Visibility == Visibility.Visible)
                {
                    LLQNotifier.Default.Register(this);
                }
            };
        }

        [SubscriberCallback(typeof(StoryExtraEvent))]
        private void StoryExtraSubscriber(StoryExtraEvent param)
        {
            CommentCount = param.StoryExtraInfo.Count.Comments.ToString();
            LikeCount = param.StoryExtraInfo.Count.Likes.ToString();
            IsLikeButtonChecked = param.StoryExtraInfo.Vote_Status == 1;
            IsFavoriteButtonChecked = param.StoryExtraInfo.Favorite;
        }

        [SubscriberCallback(typeof(OpenNewStoryEvent))]
        public void ResetSubscriber()
        {
            CommentCount = "0";
            LikeCount = "0";
            IsLikeButtonChecked = false;
            IsFavoriteButtonChecked = false;
        }

        [SubscriberCallback(typeof(LoginEvent))]
        public void LoginSubscriber(LoginEvent param)
        {
            if (param.IsLogin)
            {
                UserPhotoUrl = param.UserPhotoUrl;
                LoginSuccessButton.Visibility = Visibility.Visible;
                SettingButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserPhotoUrl = "ms-appx:///Assets/Login.png";
                LoginSuccessButton.Visibility = Visibility.Collapsed;
                SettingButton.Visibility = Visibility.Visible;
            }
        }

        [SubscriberCallback(typeof(ShareEvent))]
        private void ShareUrlSubscriber(ShareEvent shareEvent)
        {
            ShareUrl = shareEvent.ShareUrl;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Setting });
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PopupMessage.DisplayMessageInRes("Developing");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Search });
        }

        private void ShareToWeiBo(object sender, RoutedEventArgs e)
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                Animator.Use(AnimationType.Shake).PlayOn(ShareButton);
                return;
            }
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.ShareToWeiBo });
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Comment });
        }

        private void LikeStatusChanged(object sender, ToggleEventArgs e)
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                Animator.Use(AnimationType.Shake).PlayOn(LikeButton);
                e.IsCancel = true;
                return;
            }
            LikeCount = (int.Parse(LikeCount) + (e.IsChecked ? 1 : -1)).ToString();
            Animator.Use(AnimationType.StandUp).PlayOn(LikeButton);
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Like, IsChecked = e.IsChecked });
        }

        private void FavStatusChanged(object sender, ToggleEventArgs e)
        {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                Animator.Use(AnimationType.Shake).PlayOn(FavButton);
                e.IsCancel = true;
                return;
            }
            Animator.Use(AnimationType.Bounce).PlayOn(FavButton);
            LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Fav, IsChecked = e.IsChecked });
        }

        private void Notify(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
