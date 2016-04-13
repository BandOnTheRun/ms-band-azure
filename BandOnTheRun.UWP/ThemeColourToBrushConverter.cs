using MSBandAzure.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BandOnTheRun.UWP
{
    public class ThemeColourToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Colour col = (Colour)value;
            return new SolidColorBrush(Color.FromArgb(col.a, col.r, col.g, col.b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
