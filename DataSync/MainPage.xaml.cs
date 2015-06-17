using Microsoft.Band;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace DataSync
{
    public class BandData
    {
        public int HeartRate { get; set; }
        public string Timestamp { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBandClient _bandClient;
        private DeviceTelemetry _data = new DeviceTelemetry { DeviceId = "DXband" };
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }

        async void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        //    IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync(); 
        //    try 
        //    {
        //        _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);
     
        //        // do work after successful connect     
        //        // get a list of available reporting intervals 
        //        IEnumerable<TimeSpan> supportedHeartBeatReportingIntervals = _bandClient.SensorManager.HeartRate.SupportedReportingIntervals; 
        //        foreach (var ri in supportedHeartBeatReportingIntervals) 
        //        {     
        //            // do work with each reporting interval (i.e. add them to a list in the UI) 
        //        }
        //        _bandClient.SensorManager.HeartRate.ReportingInterval = supportedHeartBeatReportingIntervals.First();

        //        // hook up to the HeartRate sensor ReadingChanged event 
        //        _bandClient.SensorManager.HeartRate.ReadingChanged += async (s, args) =>  
        //        {
        //            _data.Data.HeartRate = args.SensorReading.HeartRate;
        //            _data.Data.Timestamp = args.SensorReading.Timestamp.ToString();

        //            var resp = await PostTelemetryAsync(_data);
        //            var status = resp.StatusCode;
        //            var res = await resp.Content.ReadAsStringAsync();

        //            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //                {
        //                    // do work when the reading changes (i.e. update a UI element) 
        //                    heartText.Text = args.SensorReading.HeartRate.ToString();
        //                    heartTime.Text = args.SensorReading.Timestamp.ToString();
        //                });
        //        };

        //        // start the HeartRate sensor 
        //        try
        //        {
        //            await _bandClient.SensorManager.HeartRate.StartReadingsAsync();
        //        }
        //        catch (BandException ex)
        //        {
        //            // handle a Band connection exception
        //            throw ex; 
        //        }
        //    } 
        //    catch(BandException ex) 
        //    {     // handle a Band connection exception 
        //        throw ex;
        //    }
        }

        private Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry)
        {
            var sas = "xxxxxxxxxxxxxxxxxx";

            // Namespace info.
            var serviceNamespace = "bandontherun-ns";
            var hubName = "bandontherun";
            var url = string.Format("{0}/publishers/{1}/messages", hubName, deviceTelemetry.DeviceId);

            // Create client.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace))
            };

            var payload = JsonConvert.SerializeObject(deviceTelemetry);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            content.Headers.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");
            return httpClient.PostAsync(url, content);
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
    }
}
