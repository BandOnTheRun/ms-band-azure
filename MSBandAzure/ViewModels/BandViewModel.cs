using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MSBandAzure.ViewModels
{
    public class BandViewModel : Mvvm.ViewModelBase
    {
        private Band _band;
        public BandViewModel(Band band)
        {
            _band = band;
            InitSensors();
            UpdateConnectedStatus();
        }

        public string BandName { get { return _band.Name; } }

        public HeartRateViewModel HeartRate
        {
            get
            {
                return Connected ? SensorData.OfType<HeartRateViewModel>().First() : null;
            }
        }
        private bool _isBusy;

        private void UpdateConnectedStatus()
        {
            StatusText = _band.Connected ? "Connected" : "Not Connected";
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        private Brush _tileColour = new SolidColorBrush(Color.FromArgb(255, 92, 45, 145));

        public Brush TileColour
        {
            get { return _tileColour; }
            set { SetProperty(ref _tileColour, value); }
        }

        public bool Connected { get { return _band == null ? false : _band.Connected; } }

        public async Task Connect(object arg)
        {
            if (_band.Connected)
                return;

            IsBusy = true;
            try
            {
                await _band.Connect();
                var theme = await _band.Client.PersonalizationManager.GetThemeAsync();
                TileColour = new SolidColorBrush(Color.FromArgb(255, theme.Base.R, theme.Base.G, theme.Base.B));
            }
            finally
            {
                IsBusy = false;
                UpdateConnectedStatus();
            }
        }

        private void InitSensors()
        {
            // Initialise the sensor data view models
            SensorData = new List<DataViewModelBase>
                    {
                        _band.CreateSensorViewModel<HeartRateViewModel>(),
                    };
        }

        public async Task StartSensors()
        {
            foreach (var sensor in SensorData)
            {
                var asyncCmd = sensor.StartCmd as IAsyncCommand;
                if (asyncCmd != null)
                {
                    try
                    {
                        await asyncCmd.ExecuteAsync(_band);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        public async Task StopSensors()
        {
            foreach (var sensor in SensorData)
            {
                var asyncCmd = sensor.StopCmd as IAsyncCommand;
                if (asyncCmd != null)
                {
                    try
                    {
                        await asyncCmd.ExecuteAsync(null);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        public List<DataViewModelBase> SensorData { get; set; }
    }
}
