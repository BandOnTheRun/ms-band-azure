using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSync.ViewModels
{
    public class UVViewModel : BandDataViewModel
    {
        public UVViewModel(IBandClient bandClient)
            : base("UV", bandClient)
        {
        }

        protected override bool CanStop(object arg)
        {
            return _started;
        }

        protected override async Task<object> Stop(object arg)
        {
            await _bandClient.SensorManager.UV.StopReadingsAsync();
            _bandClient.SensorManager.UV.ReadingChanged -= UV_ReadingChanged;
            _started = false;
            return _started;
        }

        protected override bool CanStart(object arg)
        {
            return !_started && !IsBusy;
        }

        protected async override Task<object> Start(object arg)
        {
            var consent = _bandClient.SensorManager.UV.GetCurrentUserConsent();
            switch (consent)
            {
                case UserConsent.NotSpecified:
                    await _bandClient.SensorManager.UV.RequestUserConsentAsync();
                    break;
                case UserConsent.Declined:
                    return false;
            }

            IsBusy = true;
            ((AsyncDelegateCommand<object>)(StartCmd)).RaiseCanExecuteChanged();

            App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.UV.ReadingChanged += UV_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.UV.StartReadingsAsync();
            }
            finally
            {
                App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        void UV_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandUVReading> e)
        {
            _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UVIndex = App.Data.UVIndex = e.SensorReading.IndexLevel;
                TimeStamp = e.SensorReading.Timestamp.UtcDateTime.ToString();
            });
        }

        private UVIndexLevel _uVIndex;
        public UVIndexLevel UVIndex
        {
            get { return _uVIndex; }
            set { SetProperty(ref _uVIndex, value); }
        }
    }
}
