using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace BandOnTheRunAlertWebJob
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Microsoft.AspNet.SignalR.Client;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            // set up service bus config
            JobHostConfiguration config = new JobHostConfiguration();
            config.UseServiceBus();

            // set up host job
            var host = new JobHost(config);

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }

    public class Functions
    {
        public static void ProcessQueueMessage(
                [ServiceBusTrigger("bandontherunqueue")] BrokeredMessage message, 
                TextWriter logger)
        {
            // get message from service bus

            var body = message.GetBody<string>();
            var alertInfo = JsonConvert.DeserializeObject<BotrAlert>(body);
            logger.WriteLine($"BandOnTheRunAlertWebJob: Processed message: {alertInfo.deviceid} - {alertInfo.heartrate}");

            // send message to signalr hub

            var connection = new HubConnection("http://bandontheruntracker.azurewebsites.net/");
            var hub = connection.CreateHubProxy("BandOnTheRunHub");
            connection.Start().Wait();

            hub.Invoke<string>("heartrate", alertInfo.deviceid, alertInfo.heartrate);
        }
    }

    public class BotrAlert
    {
        public string deviceid { get; set; }
        public int heartrate { get; set; }
        public DateTime time { get; set; }
    }

    public class JsonDeserializingMessageProvider : MessagingProvider
    {
        private readonly ServiceBusConfiguration _config;

        public JsonDeserializingMessageProvider(ServiceBusConfiguration config) : base(config)
        {
            _config = config;
        }

        public override MessageProcessor CreateMessageProcessor(string entityPath)
        {
            return new JsonDeserializingMessageProcessor(_config.MessageOptions);
        }

        private class JsonDeserializingMessageProcessor : MessageProcessor
        {
            public JsonDeserializingMessageProcessor(OnMessageOptions messageOptions)
                : base(messageOptions)
            {
            }

            public override Task<bool> BeginProcessingMessageAsync(BrokeredMessage message, CancellationToken cancellationToken)
            {
                message.ContentType = "application/json";

                return base.BeginProcessingMessageAsync(message, cancellationToken);
            }
        }
    }
}
