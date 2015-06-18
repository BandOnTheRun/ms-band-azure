using Microsoft.Band;
using Microsoft.Band.Sensors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataSync.ViewModels
{
    public class HeartRateViewModel : BandDataViewModel
    {
        public HeartRateViewModel(IBandClient bandClient)
            : base("Heart Rate", bandClient)
        {
        }

        protected override bool CanStop(object arg)
        {
            return _started;
        }

        protected override async Task<object> Stop(object arg)
        {
            await _bandClient.SensorManager.HeartRate.StopReadingsAsync();
            _bandClient.SensorManager.HeartRate.ReadingChanged -= HeartRate_ReadingChanged;
            _started = false;
            return _started;
        }

        protected override bool CanStart(object arg)
        {
            return !_started && !IsBusy;        
        }

        protected override async Task<object> Start(object arg)
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

        async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                HeartRate = App.Data.HeartRate = e.SensorReading.HeartRate;
                TimeStamp = e.SensorReading.Timestamp.UtcDateTime.ToString();
            });
        }

        //private bool _started;

        private int _heartRate;
        public int HeartRate
        {
            get { return _heartRate; }
            set { SetProperty(ref _heartRate, value); }
        }
    }

    public class SkinTempViewModel : BandDataViewModel
    {
        public SkinTempViewModel(IBandClient bandClient)
            : base("Skin Temp", bandClient)
        {
        }

        protected override bool CanStop(object arg)
        {
            return _started;
        }

        protected override async Task<object> Stop(object arg)
        {
            await _bandClient.SensorManager.SkinTemperature.StopReadingsAsync();
            _bandClient.SensorManager.SkinTemperature.ReadingChanged -= SkinTemperature_ReadingChanged;
            _started = false;
            return _started;
        }

        protected override bool CanStart(object arg)
        {
            return !_started && !IsBusy;
        }

        protected async override Task<object> Start(object arg)
        {
            var consent = _bandClient.SensorManager.SkinTemperature.GetCurrentUserConsent();
            switch (consent)
            {
                case UserConsent.NotSpecified:
                    await _bandClient.SensorManager.SkinTemperature.RequestUserConsentAsync();
                    break;
                case UserConsent.Declined:
                    return false;
            }

            IsBusy = true;
            ((AsyncDelegateCommand<object>)(StartCmd)).RaiseCanExecuteChanged();

            App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.SkinTemperature.ReadingChanged += SkinTemperature_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.SkinTemperature.StartReadingsAsync();
            }
            finally
            {
                App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        void SkinTemperature_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> e)
        {
            _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SkinTemp = App.Data.SkinTemp = e.SensorReading.Temperature;
                TimeStamp = e.SensorReading.Timestamp.UtcDateTime.ToString();
            });
        }

        private double _skinTemp;
        public double SkinTemp
        {
            get { return _skinTemp; }
            set { SetProperty(ref _skinTemp, value); }
        }
    }

    public abstract class BandDataViewModel : ViewModelBase
    {
        public BandDataViewModel(string name, IBandClient bandClient)
        {
            _bandClient = bandClient;

            _name = name;
            _dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            _startCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Start, CanStart);
            });
            _stopCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Stop, CanStop);
            });
        }

        protected bool _started;

        protected virtual bool CanStop(object arg) { return true; }
        protected abstract Task<object> Stop(object arg);

        protected virtual bool CanStart(object arg) { return true; }
        protected abstract Task<object> Start(object arg);

        Lazy<ICommand> _startCmd;
        public ICommand StartCmd { get { return _startCmd.Value; } }

        Lazy<ICommand> _stopCmd;
        public ICommand StopCmd { get { return _stopCmd.Value; } }

        protected Windows.UI.Core.CoreDispatcher _dispatcher;

        private string _name;
        public string Name { get { return _name; } }
        
        private string _timeStamp;

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        protected Microsoft.Band.IBandClient _bandClient;
    }
}
