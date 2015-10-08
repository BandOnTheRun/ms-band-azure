using Microsoft.Band;
using Microsoft.Band.Sensors;
using MSBandAzure.Mvvm;
using System;
using System.Threading.Tasks;

namespace MSBandAzure.ViewModels
{
    public class SkinTempViewModel : DataViewModelBase
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

            //App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.SkinTemperature.ReadingChanged += SkinTemperature_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.SkinTemperature.StartReadingsAsync();
            }
            finally
            {
                //App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        async void SkinTemperature_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> e)
        {
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 SkinTemp = e.SensorReading.Temperature;
                 TimeStamp = e.SensorReading.Timestamp.UtcDateTime.ToString();
             });
        }

        private double _skinTemp;
        public double SkinTemp
        {
            get { return _skinTemp; }
            set { Set(ref _skinTemp, value); }
        }
    }
}
