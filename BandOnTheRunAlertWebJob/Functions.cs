using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Client;

namespace BandOnTheRunAlertWebJob
{
    //public class Functions
    //{
    //    public static void ProcessQueueMessage([ServiceBusTrigger(queueName: "bandontherunqueue")] BrokeredMessage message, TextWriter logger)
    //    {
    //        // get message from service bus

    //        var body = message.GetBody<string>();
    //        var alertInfo = JsonConvert.DeserializeObject<BotrAlert>(body);
    //        logger.WriteLine($"BandOnTheRunAlertWebJob: Processed message: {alertInfo.deviceid} - {alertInfo.heartrate}");

    //        // send message to signalr hub

    //        var connection = new HubConnection("http://bandontheruntracker.azurewebsites.net/");
    //        var hub = connection.CreateHubProxy("BandOnTheRunHub");
    //        connection.Start().Wait();

    //        hub.Invoke<string>("heartrate", alertInfo.deviceid, alertInfo.heartrate);
    //    }
    //}

    //public class BotrAlert
    //{
    //    public string deviceid { get; set; }
    //    public int heartrate { get; set; }
    //    public DateTime time { get; set; }
    //}
}
