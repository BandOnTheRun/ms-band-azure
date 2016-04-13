using MSBandAzure.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using MSBandAzure.Mvvm;

namespace BandOnTheRun.UWP.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            DataContext = VMLocator.Instance.SettingsViewModel;
        }

        // strongly-typed view models enable x:bind
        public SettingsViewModel ViewModel => VMLocator.Instance.SettingsViewModel;
    }
}
