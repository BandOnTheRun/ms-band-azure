using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Client;

namespace AlertJob
{
    public class Functions
    {
        //// This function will get triggered/executed when a new message is written 
        //// on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
        
            //public static void WriteLogPOCO(
        //     [ServiceBusTrigger(queueName: "bandontherunqueue")]
        //    BotrAlert alertInfo,
        //     TextWriter logger)
        //{
        //    logger.WriteLine($"AlertJob: Processed message: {alertInfo.deviceid} - {alertInfo.heartrate}");
        //}

        public static async Task WriteLogPOCO(
             [ServiceBusTrigger(queueName: "bandontherunqueue")]
            BrokeredMessage alertInfoMessage,
             TextWriter logger)
        {
            var body = alertInfoMessage.GetBody<string>();

            var alertInfo = JsonConvert.DeserializeObject<BotrAlert>(body);

            logger.WriteLine($"AlertJob: Processed message: {alertInfo.deviceid} - {alertInfo.heartrate}");

            var connection = new HubConnection("http://botr-hack.azurewebsites.net/");
            var myHub = connection.CreateHubProxy("OnTheRun");

            await connection.Start();

            await myHub.Invoke<string>("Send", alertInfo.deviceid, alertInfo.heartrate);

        }
    }


    public class BotrAlert
    {
        public string deviceid { get; set; }
        public int heartrate { get; set; }
        public DateTime time { get; set; }
    }

}
