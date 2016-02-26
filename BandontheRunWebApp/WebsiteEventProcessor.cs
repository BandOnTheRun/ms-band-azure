using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace BandontheRunWebApp
{
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Text;

    public class DeviceTelemetry
    {
        public DeviceTelemetry()
        {
        }
        public string DeviceId { get; set; }

        public int HeartRate { get; set; }
        public double SkinTemp { get; set; }
        public int UVIndex { get; set; }

        public string Timestamp { get; set; }
    }



    // TODO: may remove all of this, as the website doesn't connect to IoT Hub at present, and Stream Analytics does it all

    // this is the class that handles messages from the website consumer group on the event hub

    public class WebsiteEventProcessor : IEventProcessor
    {
        // Keep track of devices seen, and the last message received for each device 

        //public static ConcurrentDictionary<string, DeviceTelemetry> _devices =
        //        new ConcurrentDictionary<string, DeviceTelemetry>();


        public static Dictionary<string, DeviceTelemetry> _devices = new Dictionary<string, DeviceTelemetry>();
        PartitionContext partitionContext;
        Stopwatch checkpointStopWatch;

        public WebsiteEventProcessor()
        {
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("SimpleEventProcessor initialize.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));
            this.partitionContext = context;
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                foreach (EventData eventData in events)
                {
                    // throw away out of date messages 

                    //if (eventData.EnqueuedTimeUtc.CompareTo(DateTime.UtcNow.AddMinutes(-15)) < 0)
                    DateTime x = DateTime.UtcNow;
                    if (eventData.EnqueuedTimeUtc.CompareTo(new DateTime(2015, 6, 25, 16, 30, 0)) < 0)
                            continue;  // enqueued date is earlier, so throw away old messages 

                    // grab payload, and this latest version in our array under the band name 

                    var newDeviceTelemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(Encoding.UTF8.GetString(eventData.GetBytes()));
                    //var newData = this.DeserializeEventData(eventData);

                    string key = newDeviceTelemetry.DeviceId;
                    _devices[key] = newDeviceTelemetry;


                }

                //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts

                if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
                {
                    await context.CheckpointAsync();
                    this.checkpointStopWatch.Restart();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error in processing: " + exp.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("Processor Shuting Down.  Partition '{0}', Reason: '{1}'.", this.partitionContext.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }
    }
}