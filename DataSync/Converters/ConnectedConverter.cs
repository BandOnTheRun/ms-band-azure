using System;
using Windows.UI.Xaml.Data;

namespace DataSync.Converters
{
    public class ConnectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
