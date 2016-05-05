using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Utils;
using Brook.DuDuRiBao.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace Brook.DuDuRiBao.Pages
{
    public sealed partial class CommentPage : Page
    {
        public CommentViewModel VM { get { return DataContext as CommentViewModel; } }

        public Visibility ToolBarVisibility { get { return Config.UIStatus == AppUIStatus.List ? Visibility.Visible : Visibility.Collapsed; } }

        private bool _isLoadComplete = false;

        public CommentPage()
        {
            this.InitializeComponent();

            CommentListView.Refresh = RefreshCommentList;
            CommentListView.LoadMore = LoadMoreComments;
            Loaded += CommentPage_Loaded;
        }

        private void CommentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModelBase.CurrentStoryId))
            {
                CommentListView.SetRefresh(true);
            }
        }

        private async void RefreshCommentList()
        {
            await VM.RequestComments(false);
            CommentListView.SetRefresh(false);
        }

        private async void LoadMoreComments()
        {
            if (_isLoadComplete)
            {
                CommentListView.FinishLoadingMore();
                return;
            }

            var preCount = VM.CommentList.Count;
            await VM.RequestComments(true);
            CommentListView.FinishLoadingMore();
            _isLoadComplete = preCount == VM.CommentList.Count;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void SendComment()
        {
            if(!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            if (string.IsNullOrEmpty(VM.PostCommentContent))
                return;

            await VM.SendComment();
            VM.PostCommentContent = "";
            CommentListView.SetRefresh(true);
        }
    }
}
