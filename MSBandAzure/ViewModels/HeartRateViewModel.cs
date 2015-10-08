using Microsoft.Band;
using Microsoft.Band.Sensors;
using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using MSBandAzure.Services;
using System;
using System.Threading.Tasks;

namespace MSBandAzure.ViewModels
{
    public class HeartRateViewModel : DataViewModelBase
    {
        private ITelemetry _telemetry;

        public HeartRateViewModel(IBandClient bandClient, ITelemetry telemetry)
            : base("Heart Rate", bandClient)
        {
            _telemetry = telemetry;
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
            var band = arg as Band;
            _bandInfo = band.Info;
            _bandClient = band.Client;
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

            //App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.HeartRate.StartReadingsAsync();
            }
            finally
            {
                //App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            var hr = e.SensorReading.HeartRate;
            var ts = e.SensorReading.Timestamp;
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 HeartRate = hr;
                 TimeStamp = ts.ToString();
             });
            await _telemetry.PostTelemetryAsync(new Models.DeviceTelemetry
            {
                DeviceId = _bandInfo.Name,
                HeartRate = hr,
                Timestamp = ts.ToString()
            });
        }

        private int _heartRate;
        private IBandInfo _bandInfo;

        public int HeartRate
        {
            get { return _heartRate; }
            set { Set(ref _heartRate, value); }
        }
    }
}
