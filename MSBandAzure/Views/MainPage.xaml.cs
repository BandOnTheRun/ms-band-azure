using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Controls;

namespace MSBandAzure.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            // Note. we must set up the DataContext as tempate 10 relies on it..
            DataContext = App.Locator.MainPageViewModel;
        }

        // strongly-typed view models enable x:bind
        public MainPageViewModel ViewModel => App.Locator.MainPageViewModel;

        private void GotoDetailsPage(object sender, ItemClickEventArgs e)
        {
            ViewModel.GotoDetailsPage(e.ClickedItem);
        }
    }
}