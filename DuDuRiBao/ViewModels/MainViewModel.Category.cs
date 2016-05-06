using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class MainViewModel
    {
        private readonly ObservableCollectionExtended<HotRiBao> _categoryList = new ObservableCollectionExtended<HotRiBao>();

        public ObservableCollectionExtended<HotRiBao> CategoryList { get { return _categoryList; } }

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

        public async void InitCategories()
        {
            var categories = await DataRequester.RequestCategory();
            if (categories == null)
                return;

            CategoryList.AddRange(categories.others);

            if (CategoryList.Count > 0)
            {
                CategoryName = CategoryList[0].Title;
            }
        }
    }
}
