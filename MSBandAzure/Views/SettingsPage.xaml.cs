using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace MSBandAzure.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            DataContext = App.Locator.SettingsViewModel;
        }

        // strongly-typed view models enable x:bind
        public SettingsViewModel ViewModel => App.Locator.SettingsViewModel;
    }
}
