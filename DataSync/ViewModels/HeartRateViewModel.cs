using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private int _heartRate;
        public int HeartRate
        {
            get { return _heartRate; }
            set { SetProperty(ref _heartRate, value); }
        }
    }
}
