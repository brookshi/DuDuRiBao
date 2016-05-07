using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;

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

        public async void InitHotCircles()
        {
            var hotCircles = await DataRequester.RequestHotCircles();
            if (string.IsNullOrEmpty(hotCircles))
                return;

            HotCircles.AddRange(HotCircleBuilder.Builder(hotCircles));
        }
    }
}
