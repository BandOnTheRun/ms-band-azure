using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataSync.ViewModels
{
    public class HeartRateViewModel : BandDataViewModel
    {
        public HeartRateViewModel(Microsoft.Band.IBandClient bandClient)
            : base("Heart Rate")
        {
            _bandClient = bandClient;
            _startCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Start, CanStart);
            });
            _stopCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Stop, CanStop);
            });
            _dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
        }

        private bool CanStop(object arg)
        {
            return _started;
        }

        private async Task<object> Stop(object arg)
        {
            await _bandClient.SensorManager.HeartRate.StopReadingsAsync();
            _bandClient.SensorManager.HeartRate.ReadingChanged -= HeartRate_ReadingChanged;
            _started = false;
            return _started;
        }

        private bool CanStart(object arg)
        {
            return !_started && !IsBusy;        
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private async Task<object> Start(object arg)
        {
            var consent = _bandClient.SensorManager.HeartRate.GetCurrentUserConsent();
            switch (consent)
            {
                case UserConsent.NotSpecified:
                    await _bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                    break;
                case UserConsent.Declined:
                    return false;
            }

            IsBusy = true;
            ((AsyncDelegateCommand<object>)(StartCmd)).RaiseCanExecuteChanged();

            App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.HeartRate.StartReadingsAsync();
            }
            finally
            {
                App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                HeartRate = e.SensorReading.HeartRate;
                TimeStamp = e.SensorReading.Timestamp.UtcDateTime.ToString();
            });
        }

        private bool _started;

        Lazy<ICommand> _startCmd;
        public ICommand StartCmd { get { return _startCmd.Value; } }

        Lazy<ICommand> _stopCmd;
        public ICommand StopCmd { get { return _stopCmd.Value; } }

        public string MyProperty { get { return "test"; } }
        private int _heartRate;
        private Microsoft.Band.IBandClient _bandClient;
        private Windows.UI.Core.CoreDispatcher _dispatcher;

        public int HeartRate
        {
            get { return _heartRate; }
            set { SetProperty(ref _heartRate, value); }
        }
    }

    public class BandDataViewModel : ViewModelBase
    {
        public BandDataViewModel(string name)
        {
            _name = name;
        }

        private string _name;
        public string Name { get { return _name; } }
        
        private string _timeStamp;

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }
    }
}
