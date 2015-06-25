using DataSync.Common;
using DataSync.ViewModels;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBandClient _bandClient;
        private DeviceTelemetry _data = new DeviceTelemetry { DeviceId = "DXband" };
        private NavigationHelper navigationHelper;

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null)
                return;
            var band = vm.ConnectedBand;
            if (band == null)
                return;

            e.PageState.Add("ConnectedBand", band.Info.Name);
        }

        async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null)
                return;
            var band = vm.ConnectedBand;
            if (band == null)
            {
                object bandName = null;
                if (e.PageState != null && e.PageState.TryGetValue("ConnectedBand", out bandName))
                {
                    // try to connect to the band...
                    await vm.ConnectToBandCmd.ExecuteAsync((string)bandName);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
