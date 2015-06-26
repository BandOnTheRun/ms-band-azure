using DataSync.Common;
using DataSync.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace DataSync
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;

        public MainPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
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
                    await vm.ConnectToBandCmd.ExecuteAsync(bandName);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }
    }
}
