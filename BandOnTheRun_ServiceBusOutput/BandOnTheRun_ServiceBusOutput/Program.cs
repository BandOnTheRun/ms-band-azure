using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Configuration;

namespace BandOnTheRun_ServiceBusOutput
{
    class Program
    {

        static void Main(string[] args)
        {
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists("BandOnTheRun"))
            {
                namespaceManager.CreateQueue("BandOnTheRun");
            }

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, "BandOnTheRun");
            
            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;

            // Callback to handle received messages.
                            client.OnMessage((message) =>
                            {
                                try
                                {
                                    // Process message from queue.
                                    Console.WriteLine("Body: " + message.GetBody<string>());
                                    Console.WriteLine("MessageID: " + message.MessageId);

                                    // Remove message from queue.
                                    message.Complete();
                                    Console.WriteLine("Message completed");
                                }
                                catch (Exception)
                                {
                                    // Indicates a problem, unlock message in queue.
                                    message.Abandon();
                                    Console.WriteLine("Message error");
                                }
                            }, options);
            

            

        }
    }
}
