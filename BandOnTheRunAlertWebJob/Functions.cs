using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Client;

namespace BandOnTheRunAlertWebJob
{
    public class Functions
    {
        public static async Task WriteLogPOCO([ServiceBusTrigger(queueName: "bandontherunqueue")]
                                              BrokeredMessage alertInfoMessage, TextWriter logger)
        {
            var body = alertInfoMessage.GetBody<string>();
            var alertInfo = JsonConvert.DeserializeObject<BotrAlert>(body);

            logger.WriteLine($"AlertJob: Processed message: {alertInfo.deviceid} - {alertInfo.heartrate}");

            var connection = new HubConnection("http://bandontheruntracker.azurewebsites.net/");
            var myHub = connection.CreateHubProxy("BandOnTheRunHub");

            await connection.Start();
            await myHub.Invoke<string>("Heartrate", alertInfo.deviceid, alertInfo.heartrate);
        }
    }

    public class BotrAlert
    {
        public string deviceid { get; set; }
        public int heartrate { get; set; }
        public DateTime time { get; set; }
    }
}
