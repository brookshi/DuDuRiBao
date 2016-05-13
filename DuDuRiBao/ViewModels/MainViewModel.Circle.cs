using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using XP;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class MainViewModel
    {
        private List<HotCircle> _hotCircles = new List<HotCircle>();

        public List<HotCircle> HotCircles
        {
            get { return _hotCircles; }
            set { _hotCircles = value; Notify("HotCircles"); }
        }

        private int _currentCategoryId = Misc.Default_Category_Id;
        public int CurrentCategoryId
        {
            get { return _currentCategoryId; }
            set
            {
                if (value != _currentCategoryId)
                {
                    _currentCategoryId = value;
                    Notify("CurrentCategoryId");
                }
            }
        }

        private string _categoryName = StringUtil.GetString("DefaultCategory");
        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (value != _categoryName)
                {
                    _categoryName = value;
                    Notify("CategoryName");
                }
            }
        }

        public async void RefreshHotCircles()
        {
            var hotCircleList = await DataRequester.RequestHotCircles().ContinueWith(hotCirclesTask =>
            {
                var hotCircles = hotCirclesTask.Result;
                if (string.IsNullOrEmpty(hotCircles))
                    return null;
                return HotCircleBuilder.Builder(hotCircles);
            }) ;

            if(hotCircleList != null)
            {
                hotCircleList.ForEach(circle => circle.Adjust());
                HotCircles = hotCircleList;
            }
        }

        public DelayCommand<XPButton> JoinQuitCircleCommand { get; set; } = new DelayCommand<XPButton>(btn => {
            if (!AuthorizationHelper.IsLogin)
            {
                PopupMessage.DisplayMessageInRes("NeedLogin");
                return;
            }

            var circle = btn.DataContext as HotCircle;
            if (circle == null)
                return;

            if (btn.Tag.ToString() == "0")
            {
                DataRequester.JoinCircle(circle.Id);
                btn.Icon = new SymbolIcon(Symbol.Accept);
                btn.PointerOverIconForeground = btn.PressedIconForeground = btn.IconForeground = ResUtil.GetAppThemeBrush("BrushCheckedForeground");
                btn.Tag = "1";
            }
            else
            {
                DataRequester.QuitCircle(circle.Id);
                btn.Icon = new SymbolIcon(Symbol.Add);
                btn.PointerOverIconForeground = btn.PressedIconForeground = btn.IconForeground = ResUtil.GetAppThemeBrush("BrushButtonForeground");
                btn.Tag = "0";
            }
        });
    }
}
