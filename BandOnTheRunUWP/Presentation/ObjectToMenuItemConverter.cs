﻿using System;
using Windows.UI.Xaml.Data;

namespace MSBandAzure.Presentation
{
    public class ObjectToMenuItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
                return (MenuItem)value;
            return value;
        }
    }
}
