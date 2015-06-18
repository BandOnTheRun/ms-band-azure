﻿using DataSync.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DataSync
{
    public class SensorDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeartRateTemplate { get; set; }
        public DataTemplate SkinTempTemplate { get; set; }
        public DataTemplate UVTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is HeartRateViewModel)
                return HeartRateTemplate;
            if (item is SkinTempViewModel)
                return SkinTempTemplate;
            if (item is UVViewModel)
                return UVTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
