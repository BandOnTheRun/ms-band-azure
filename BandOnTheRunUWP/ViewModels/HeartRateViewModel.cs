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
using System.Collections;
using System.Diagnostics;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


namespace MSBandAzure.ViewModels
{
    public class FixedSizedQueue<T> : IEnumerable<T>
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

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }

    public class HeartRateViewModel : DataViewModelBase
    {
        public static readonly int BufferSize = 20; 
        private ITelemetry _telemetry;
        private IEventAggregator _events;

        public HeartRateViewModel(IBandClient bandClient, ITelemetry telemetry, IEventAggregator events)
            : base("Heart Rate", bandClient)
        {
            _telemetry = telemetry;
            _events = events;
            _hrv = new HeartRateValueUpdated { ViewModel = this };

            // Pre-roll some data..
            _hrData.Enqueue(new HeartRateValue() { Value = 200 });
            _hrData.Enqueue(new HeartRateValue() { Value = 193 });
            _hrData.Enqueue(new HeartRateValue() { Value = 186 });
            _hrData.Enqueue(new HeartRateValue() { Value = 179 });
            _hrData.Enqueue(new HeartRateValue() { Value = 172 });
            _hrData.Enqueue(new HeartRateValue() { Value = 165 });
            _hrData.Enqueue(new HeartRateValue() { Value = 158 });
            _hrData.Enqueue(new HeartRateValue() { Value = 151 });
            _hrData.Enqueue(new HeartRateValue() { Value = 144 });
            _hrData.Enqueue(new HeartRateValue() { Value = 137 });
            _hrData.Enqueue(new HeartRateValue() { Value = 130 });
            _hrData.Enqueue(new HeartRateValue() { Value = 123 });
            _hrData.Enqueue(new HeartRateValue() { Value = 116 });
            _hrData.Enqueue(new HeartRateValue() { Value = 109 });
            _hrData.Enqueue(new HeartRateValue() { Value = 102 });
            _hrData.Enqueue(new HeartRateValue() { Value = 95 });
            _hrData.Enqueue(new HeartRateValue() { Value = 88 });
            _hrData.Enqueue(new HeartRateValue() { Value = 81 });
            _hrData.Enqueue(new HeartRateValue() { Value = 74 });
            _hrData.Enqueue(new HeartRateValue() { Value = 67 });
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
                //Debug.WriteLine(Task.CurrentId);
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

        static Models.DeviceTelemetry _data = new Models.DeviceTelemetry();
         
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var hr = e.SensorReading.HeartRate;
            var ts = e.SensorReading.Timestamp;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _data.DeviceId = _bandInfo.Name;
            _data.HeartRate = hr;
            _data.Timestamp = ts.ToString();

            _telemetry.PostTelemetryAsync(
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _data);

            //UpdateHistory(hr);
            //_events.Publish(_hrv);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                 HeartRate = hr;
                 TimeStamp = ts.ToString();

                //increment notification timer
                NotificationTimer++;

#if USE_NOTIFICATIONS
                //increment notification timer
                NotificationTimer++;
                if ((NotificationTimer%30)==0)
                {
                    //Display notification for heart rate
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(_bandInfo.Name + " is working hard!\nHeart Rate: " + HeartRate + " bpm"));

                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/tpoc-heart-logo-hi.png");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "heart");

                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((XmlElement)toastNode).SetAttribute("duration", "short");
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
        
                    NotificationTimer = 0;
                }
#endif
            });

            // Only post telemetry if the heart rate is currently locked - also, only send every two readings
            // (limit of 8000 messages per day in IOT hub..)
            //if (e.SensorReading.Quality == HeartRateQuality.Acquiring || (NotificationTimer+1)%2 == 0)
            //    return;

        }

        private int _heartRate;
        private IBandInfo _bandInfo;

        public int HeartRate
        {
            get { return _heartRate; }
            set { SetProperty(ref _heartRate, value); }
        }

        private FixedSizedQueue<HeartRateValue> _hrData = new FixedSizedQueue<HeartRateValue>(BufferSize);

        public IEnumerable<HeartRateValue> HrData
        {
            get { return _hrData; }
        }

        public long NotificationTimer { get; set; }

        public void UpdateHistory(int newValue)
        {
            _hrData.Enqueue(new HeartRateValue { Value = newValue });
            return;
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
