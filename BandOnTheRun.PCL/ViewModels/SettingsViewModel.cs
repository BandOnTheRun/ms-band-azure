using BandOnTheRun.PCL.Services;
using MSBandAzure.Mvvm;

namespace MSBandAzure.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private ISettingsService _settings;
        public SettingsViewModel(ISettingsService settings)
        {
            _settings = settings;
        }

        public bool AutoConnect
        {
            get
            {
                return _settings.AutoConnect;
            }
            set
            {
                if (_settings.AutoConnect != value)
                {
                    _settings.AutoConnect = value;
                }
            }
        }
    }
}
