using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using XP;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class MainViewModel
    {
        private readonly ObservableCollectionExtended<HotCircle> _hotCircles = new ObservableCollectionExtended<HotCircle>();

        public ObservableCollectionExtended<HotCircle> HotCircles { get { return _hotCircles; } }

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

        private string _categoryName = "";
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
                HotCircles.Clear();
                HotCircles.AddRange(hotCircleList);
            }
        }

        public DelayCommand<HotCircle> AddCircleCommand { get; set; } = new DelayCommand<HotCircle>(circle => {
            System.Diagnostics.Debug.WriteLine("aa");
        });
    }
}
