using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace MSBandAzure.Views
{
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            DataContext = App.Locator.DetailPageViewModel;
        }

    // strongly-typed view models enable x:bind
    public DetailPageViewModel ViewModel => DataContext as DetailPageViewModel;
    }
}
