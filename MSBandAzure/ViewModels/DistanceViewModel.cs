using Microsoft.Band;
using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBandAzure.ViewModels
{
    public class DistanceViewModel : DataViewModelBase
    {
        public DistanceViewModel(IBandClient bandClient)
            : base("Distance", bandClient)
        {
        }

        protected override bool CanStop(object arg)
        {
            return _started;
        }

        protected override async Task<object> Stop(object arg)
        {
            await _bandClient.SensorManager.Distance.StopReadingsAsync();
            _bandClient.SensorManager.Distance.ReadingChanged -= Distance_ReadingChanged;
            _started = false;
            return _started;
        }

        protected override bool CanStart(object arg)
        {
            return !_started && !IsBusy;
        }

        private IBandInfo _bandInfo;

        protected async override Task<object> Start(object arg)
        {
            var band = arg as Band;
            _bandInfo = band.Info;
            _bandClient = band.Client;

            var consent = _bandClient.SensorManager.Distance.GetCurrentUserConsent();
            switch (consent)
            {
                case UserConsent.NotSpecified:
                    await _bandClient.SensorManager.Distance.RequestUserConsentAsync();
                    break;
                case UserConsent.Declined:
                    return false;
            }

            IsBusy = true;
            ((AsyncDelegateCommand<object>)(StartCmd)).RaiseCanExecuteChanged();

            //App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Starting..." });

            try
            {
                _bandClient.SensorManager.Distance.ReadingChanged += Distance_ReadingChanged;
                // If the user consent was granted
                _started = await _bandClient.SensorManager.Distance.StartReadingsAsync();
            }
            finally
            {
                //App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
                IsBusy = false;
                ((AsyncDelegateCommand<object>)(StopCmd)).RaiseCanExecuteChanged();
            }
            return _started;
        }

        private async void Distance_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandDistanceReading> e)
        {
            await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CurrentMotion = e.SensorReading.CurrentMotion.ToString();
                Distance = Math.Round((double)e.SensorReading.TotalDistance, 2);
                Pace = Math.Round((double)e.SensorReading.Pace, 2);
                Speed = Math.Round((double)e.SensorReading.Speed, 2);
            });
        }

        private double _speed;

        public double Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); }
        }

        private double _pace;

        public double Pace
        {
            get { return _pace; }
            set { SetProperty(ref _pace, value); }
        }

        private string _currentMotion;

        public string CurrentMotion
        {
            get { return _currentMotion; }
            set { SetProperty(ref _currentMotion, value); }
        }

        private double _distance;
        public double Distance
        {
            get { return _distance; }
            set { SetProperty(ref _distance, value); }
        }
    }
}
