using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band;
using System.Diagnostics;
using MSBandAzure.Services;

namespace MSBandAzure.ViewModels
{
    public class Colour
    {
        private byte a, r, g, b;

        public static Colour FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Colour { a = a, r = r, g = g, b = b };
        }
    }
    public class BandThemeViewModel : ViewModelBase
    {
        private Colour _tileColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour TileColour
        {
            get { return _tileColour; }
            set { SetProperty(ref _tileColour, value); }
        }

        private Colour _highlightColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour HighlightColour
        {
            get { return _highlightColour; }
            set { SetProperty(ref _highlightColour, value); }
        }

        private Colour _lowlightColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour LowlightColour
        {
            get { return _lowlightColour; }
            set { SetProperty(ref _lowlightColour, value); }
        }

        private Colour _highContrastColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour HighContrastColour
        {
            get { return _highContrastColour; }
            set { SetProperty(ref _highContrastColour, value); }
        }

        private Colour _mutedColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour MutedColour
        {
            get { return _mutedColour; }
            set { SetProperty(ref _mutedColour, value); }
        }

        private Colour _secondaryTextColour = Colour.FromArgb(255, 92, 45, 145);

        public Colour SecondaryTextColour
        {
            get { return _secondaryTextColour; }
            set { SetProperty(ref _secondaryTextColour, value); }
        }

        internal void SetBandTheme(BandTheme theme)
        {
            LowlightColour = Colour.FromArgb(255,
                theme.Lowlight.R, theme.Lowlight.G, theme.Lowlight.B);
            HighlightColour = Colour.FromArgb(255,
                theme.Highlight.R, theme.Highlight.G, theme.Highlight.B);
            TileColour = Colour.FromArgb(255,
                theme.Base.R, theme.Base.G, theme.Base.B);
            HighContrastColour = Colour.FromArgb(255,
                theme.HighContrast.R, theme.HighContrast.G, theme.HighContrast.B);
            MutedColour = Colour.FromArgb(255,
                theme.Muted.R, theme.Muted.G, theme.Muted.B);
            SecondaryTextColour = Colour.FromArgb(255,
                theme.SecondaryText.R, theme.SecondaryText.G, theme.SecondaryText.B);
        }
    }

    public class BandViewModel : Mvvm.ViewModelBase
    {
        private BandThemeViewModel _theme = new BandThemeViewModel();

        public BandThemeViewModel Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        private Band _band;
        public BandViewModel(Band band, ITelemetry telemetry)
        {
            _telemetry = telemetry;
            _band = band;
            InitSensors();
            UpdateConnectedStatus();
            _connectCmd = new Lazy<AsyncDelegateCommand<object>>(() =>
            {
                return new AsyncDelegateCommand<object>(ConnectBand, CanConnect);
            });
        }

        private ITelemetry _telemetry;

        private async Task<object> ConnectBand(object arg)
        {
            try
            {
                VMLocator.Instance.CurrentBand = this;
                IsBusy = true;
                await Connect(arg);
                await StartSensors();

                //TODO: This needs to have a telemetry instance per-band instance (it is currently a singleton!!)
                //_telemetry = App.Locator.Resolve<ITelemetry>();
                await _telemetry.RefreshTokenAsync(BandName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }

            return arg;
        }

        Lazy<AsyncDelegateCommand<object>> _connectCmd;
        public AsyncDelegateCommand<object> ConnectCmd { get { return _connectCmd.Value; } }

        private bool CanConnect(object arg)
        {
            return true;
        }

        public string BandName { get { return _band.Name; } }

        public HeartRateViewModel HeartRate
        {
            get
            {
                return SensorData.OfType<HeartRateViewModel>().First();
            }
        }

        public DistanceViewModel Distance
        {
            get
            {
                return SensorData.OfType<DistanceViewModel>().First();
            }
        }

        public SkinTempViewModel SkinTemp
        {
            get
            {
                return SensorData.OfType<SkinTempViewModel>().First();
            }
        }

        public UVViewModel UV
        {
            get
            {
                return SensorData.OfType<UVViewModel>().First();
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


        public bool Connected { get { return _band == null ? false : _band.Connected; } }

        public async Task Connect(object arg)
        {
            if (_band.Connected)
                return;

            IsBusy = true;
            try
            {
                await _band.Connect();
                SetupThemeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                UpdateConnectedStatus();
            }
        }

        private Task SetupThemeAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            var themeTask = _band.Client.PersonalizationManager.GetThemeAsync();
            themeTask.ContinueWith(t => 
                {
                    var theme = t.Result;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _dispatcher.RunAsync(() =>
                    {
                        Theme.SetBandTheme(theme);
                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                });

            var meTileTask = _band.Client.PersonalizationManager.GetMeTileImageAsync();
            Task.WhenAll(themeTask, meTileTask);

            return tcs.Task;
        }

        private void InitSensors()
        {
            // Initialise the sensor data view models
            SensorData = new List<DataViewModelBase>
                    {
                        _band.CreateSensorViewModel<HeartRateViewModel>(_telemetry),
                        _band.CreateSensorViewModel<SkinTempViewModel>(_telemetry),
                        _band.CreateSensorViewModel<UVViewModel>(_telemetry),
                        _band.CreateSensorViewModel<DistanceViewModel>(_telemetry)
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
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception: {ex.Message}");
                    }
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
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception: {ex.Message}");
                    }
                }
            }
        }

        public List<DataViewModelBase> SensorData { get; set; }
    }
}
