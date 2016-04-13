using Caliburn.Micro;
using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MSBandAzure.Views
{
    public sealed partial class HeartRateChartControl : UserControl, IHandle<HeartRateValueUpdated>
    {
        public HeartRateChartControl()
        {
            var events = App.Locator.Resolve<IEventAggregator>();
            events.Subscribe(this);
            this.InitializeComponent();

            Loaded += HeartRateChartControl_Loaded;
            Unloaded += HeartRateChartControl_Unloaded;
        }

        public bool IsLoaded { get; set; }

        private void HeartRateChartControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsLoaded = false;
        }

        private bool axisLabelsHidden = false;
        private void HeartRateChartControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!this.axisLabelsHidden)
            {
                var series = (columnChart.Series[0] as ColumnSeries);
                series.DependentRangeAxis =
                    new LinearAxis
                    {
                        Minimum = 60,
                        Maximum = 200,
                        Orientation = AxisOrientation.Y,
                        Interval = 20,
                        ShowGridLines = false,
                        Width = 0
                    };
                series.IndependentAxis =
                    new CategoryAxis
                    {
                        Orientation = AxisOrientation.X,
                        Height = 0
                    };
                this.axisLabelsHidden = true;
            }

            IsLoaded = true;
        }

        // Pre-allocate list to store the chart data - note. 
        private HeartRateValue[] ChartData = new HeartRateValue[HeartRateViewModel.BufferSize];
         
        public async void Handle(HeartRateValueUpdated message)
        {
            if (IsLoaded == false)
                return;

            Debug.WriteLine($"Handling HR Changed {DateTime.Now.Second}");

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var band = DataContext as BandViewModel;
                // if the bands match update the chart..
                if (band.HeartRate == message.ViewModel)
                {
                    //message.ViewModel.CopyTo(ChartData);

                    // copy the data...
                    var cs = (columnChart.Series[0] as ColumnSeries);
                    //cs.ItemsSource = null;
                    cs.ItemsSource = message.ViewModel.HrData;
                    cs.Refresh();
                }
            });
        }
    }
}
