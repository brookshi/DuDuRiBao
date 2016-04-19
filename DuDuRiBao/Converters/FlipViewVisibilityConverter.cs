using Brook.DuDuRiBao.Utils;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Brook.DuDuRiBao.Converters
{
    public class FlipViewVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == Misc.Favorite_Category_Id ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
