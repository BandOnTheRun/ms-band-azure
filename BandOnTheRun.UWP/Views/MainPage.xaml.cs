using MSBandAzure.Mvvm;
using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Controls;

namespace BandOnTheRun.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            // Note. we must set up the DataContext as template 10 relies on it..
            DataContext = VMLocator.Instance.MainPageViewModel;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => VMLocator.Instance.MainPageViewModel;

        private void GotoDetailsPage(object sender, ItemClickEventArgs e)
        {
            ViewModel.GotoDetailsPage(e.ClickedItem);
        }

        private void widerListsizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)widerList.ItemsPanelRoot;
            panel.ItemWidth = e.NewSize.Width / 2;
            //panel.ItemHeight = e.NewSize.Height / 3;
        }
    }
}