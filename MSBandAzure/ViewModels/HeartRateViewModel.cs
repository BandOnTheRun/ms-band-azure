using Caliburn.Micro;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using MSBandAzure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSBandAzure.ViewModels
{
    public class FixedSizedQueue<T>
    {
        private readonly object privateLockObject = new object();

        readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public void Enqueue(T obj)
        {
            queue.Enqueue(obj);

            lock (privateLockObject)
            {
                while (queue.Count > Size)
                {
                    T outObj;
                    queue.TryDequeue(out outObj);
                }
            }
        }
    }

    public class HeartRateViewModel : DataViewModelBase
    {
        private ITelemetry _telemetry;
        private IEventAggregator _events;

        public HeartRateViewModel(IBandClient bandClient, ITelemetry telemetry, IEventAggregator events)
            : base("Heart Rate", bandClient)
        {
            _telemetry = telemetry;
            _events = events;
            _hrv = new HeartRateValueUpdated { ViewModel = this };
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

        private HeartRateValueUpdated _hrv;
         
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

            UpdateHistory(hr);
            _events.Publish(_hrv);

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
            set { SetProperty(ref _heartRate, value); }
        }

        private List<HeartRateValue> _data = new List<HeartRateValue>()
        {
            new HeartRateValue() { Value = 200 },
            new HeartRateValue() { Value = 193 },
            new HeartRateValue() { Value = 186 },
            new HeartRateValue() { Value = 179 },
            new HeartRateValue() { Value = 172 },
            new HeartRateValue() { Value = 165 },
            new HeartRateValue() { Value = 158 },
            new HeartRateValue() { Value = 151 },
            new HeartRateValue() { Value = 144 },
            new HeartRateValue() { Value = 137 },
            new HeartRateValue() { Value = 130 },
            new HeartRateValue() { Value = 123 },
            new HeartRateValue() { Value = 116 },
            new HeartRateValue() { Value = 109 },
            new HeartRateValue() { Value = 102 },
            new HeartRateValue() { Value = 95 },
            new HeartRateValue() { Value = 88 },
            new HeartRateValue() { Value = 81 },
            new HeartRateValue() { Value = 74 },
            new HeartRateValue() { Value = 67 },
        };

        public List<HeartRateValue> Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        public void UpdateHistory(int newValue)
        {
            // add a value to the list and move one off...
            if (Data.Count > 20)
            {
                Data.RemoveAt(0);
            }
            // FIXME: no need to new these up each time...
            Data.Add(new HeartRateValue { Value = newValue });
        }
    }

    public class HeartRateValueUpdated
    {
        public HeartRateViewModel ViewModel { get; set; }
    }

    public class HeartRateValue
    {
        public int Value { get; set; }
        public string Name { get { return ""; } }
    }
}
