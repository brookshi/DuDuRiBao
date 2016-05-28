using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using DuDuRiBao.Utils;
using LLM;
using LLQ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XP;

namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class Searcher : UserControl, INotifyPropertyChanged
    {
        Dictionary<int, bool> _updateStatus = new Dictionary<int, bool>();

        private IList<SearchCircle> _searchCircles = new List<SearchCircle>();
        public IList<SearchCircle> SearchCircles
        {
            get { return _searchCircles; }
            set
            {
                _searchCircles = value;
                Notify("SearchCircles");
            }
        }

        private IList<SearchStory> _searchStories = new List<SearchStory>();
        public IList<SearchStory> SearchStories
        {
            get { return _searchStories; }
            set
            {
                _searchStories = value;
                Notify("SearchStories");
            }
        }

        public Searcher()
        {
            this.InitializeComponent();
            SearchCircleListView.DataContext = this;
        }

        public DelayCommand<XPButton> JoinQuitCircleCommand { get; set; } = new DelayCommand<XPButton>(async btn => {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            var circle = btn.DataContext as SearchCircle;
            if (circle == null)
                return;

            if (btn.Tag.ToString() == "0")
            {
                await DataRequester.JoinCircle(circle.Id);
                btn.Content = StringUtil.GetString("GetCircle");
                btn.PointerOverBackground = ResUtil.GetAppThemeBrush("BrushPointerOver");
                btn.PressedBackground = ResUtil.GetAppThemeBrush("BrushPressed");  
                btn.Background = ResUtil.GetAppThemeBrush("BrushPointerOver");
                btn.Tag = "1";
            }
            else
            {
                await DataRequester.QuitCircle(circle.Id);
                btn.Content = StringUtil.GetString("AddCircle.Content");
                btn.PointerOverBackground = ResUtil.GetAppThemeBrush("BrushPrimaryLightS");
                btn.PressedBackground = ResUtil.GetAppThemeBrush("BrushPrimaryDark");
                btn.Background = ResUtil.GetAppThemeBrush("BrushPrimary");
                btn.Tag = "0";
            }
        });

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Animator.Use(AnimationType.FadeOutUp).SetDuration(TimeSpan.FromMilliseconds(300)).PlayOn(this, () => Visibility = Visibility.Collapsed);
        }

        private void SearchStoryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var story = e.ClickedItem as SearchStory;
            if (story == null)
                return;

            LLQNotifier.Default.Notify(new SearchEvent() { SearchObj = story, Type = SearchType.Story });
        }

        private void SearchCircleListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var circle = e.ClickedItem as SearchCircle;
            if (circle == null)
                return;

            LLQNotifier.Default.Notify(new SearchEvent() { SearchObj = circle, Type = SearchType.Circle });
        }

        private void SearchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            _updateStatus.Clear();
            var text = ((TextBox)sender).Text;
            System.Diagnostics.Debug.WriteLine(text);
            if (text == "")
            {
                SearchCircles = new List<SearchCircle>();
                SearchStories = new List<SearchStory>();
                return;
            }

            if (SearchPivot.SelectedIndex == 0)
            {
                UpdateCircles(text);
            }
            else if(SearchPivot.SelectedIndex == 1)
            {
                UpdateStories(text);
            }
        }

        private void SearchPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = SearchTxt.Text;
            if (text == "")
                return;

            UpdateStories(text);
            UpdateCircles(text);
        }

        async void UpdateStories(string text)
        {
            if (_updateStatus.ContainsKey(1) && _updateStatus[1])
                return;

            _updateStatus[1] = true;
            var stories = await DataRequester.SearchStories(text);
            if (stories != null && stories.Stories != null)
            {
                var list = stories.Stories.ToList();
                list.ForEach(o => o.Adjust());
                SearchStories = list;
            }
        }

        async void UpdateCircles(string text)
        {
            if (_updateStatus.ContainsKey(0) && _updateStatus[0])
                return;

            _updateStatus[0] = true;
            var circles = await DataRequester.SearchCircles(text);
            if (circles != null && circles.Circles != null)
            {
                var list = circles.Circles.ToList();
                list.ForEach(o => o.Adjust());
                SearchCircles = list;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void SearchPivot_Loaded(object sender, RoutedEventArgs e)
        {
            var headerTxts = VisualHelper.FindVisualChilds<TextBlock>(SearchPivot, "HeaderTxt");
            if (headerTxts != null)
            {
                headerTxts.ForEach(headerTxt => headerTxt.Width = Math.Max(SearchPivot.ActualWidth / 2 - 24, 0));
            }
        }

        public void FocusText()
        {
            SearchTxt.Focus(FocusState.Programmatic);
        }
    }
}
