using MSBandAzure.Mvvm;
using Windows.Storage;

namespace MSBandAzure.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            var ac = roamingSettings.Values["AutoConnect"];
            _autoConnect = (ac == null || (bool)ac == false) ? false : true;
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }

        private void Current_DataChanged(ApplicationData sender, object args)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            var ac = roamingSettings.Values["AutoConnect"];
            AutoConnect = (ac == null) ? false : true;
        }

        private bool _autoConnect;

        public bool AutoConnect
        {
            get
            {
                return _autoConnect;
            }
            set
            {
                if (SetProperty(ref _autoConnect, value) == true)
                {
                    ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
                    roamingSettings.Values["AutoConnect"] = value;
                }
            }
        }
    }
}
