using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace AlertJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            JobHostConfiguration config = new JobHostConfiguration();

            //var serviceBusConfig = new ServiceBusConfiguration();
            //serviceBusConfig.MessagingProvider = new JsonDeserializingMessageProvider(serviceBusConfig);
            //config.UseServiceBus(serviceBusConfig);

            config.UseServiceBus();

            JobHost host = new JobHost(config);

            host.RunAndBlock();
        }
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
