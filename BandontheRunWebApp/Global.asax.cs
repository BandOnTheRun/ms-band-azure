using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Diagnostics;


namespace BandontheRunWebApp
{
    using Microsoft.ServiceBus.Messaging;  // Microsoft Azure Service Bus Event Hub - EventProcessorHost Nuget 
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;


    public struct EventHubConfigInfo
    {
        public string eventHubNamespace { get; set; }
        public string eventHubPath { get; set; }
        public string SendKeyName { get; set; }
        public string SendKeyValue { get; set; }
        public string consumerGroup { get; set; }
        public string eventHubConnectionString { get; set; }
        public string storageConnectionString { get; set; } 
        public string iotHubConnectionString { get; set; }
        public EventProcessorHost processorHost { get; set; } 

    }
    


    public class WebApiApplication : System.Web.HttpApplication
    {
        public static EventHubConfigInfo ehWebConsumerGroup;
        

        protected void Application_Start()
        {
            // standard config 

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);  // web api config
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            // set up EventProcessorHost consumer group

            GetEventHubConsumerGroupConfig(ref ehWebConsumerGroup);
            CreateEventProcessorHostClient(ref ehWebConsumerGroup);
        }


        // create the EventProcessorHost consumer group that we will listen to data from our event hub
        private void CreateEventProcessorHostClient(ref EventHubConfigInfo eventHubSettings)
        {
            try
            {
                // we use the EventProcessorHost class to processes data from the Event Hub
                
                eventHubSettings.processorHost = new EventProcessorHost(
                        this.Server.MachineName,
                        eventHubSettings.eventHubPath,
                        eventHubSettings.consumerGroup.ToLowerInvariant(),
                        eventHubSettings.eventHubConnectionString,
                        eventHubSettings.storageConnectionString);
                
                // could now add eventHubSettings.processorHostOptions if needed
                
                // wire up our class to handle events from event hub
                eventHubSettings.processorHost.RegisterEventProcessorAsync<WebsiteEventProcessor>().Wait();
            }
            catch (Exception e)
            {
                Debug.Print("Error happened while trying to connect Event Hub - {0}", e.Message);
            }
        }



        // read the BandontheRun.XXX related config from Azure portal or web.config
        private void GetEventHubConsumerGroupConfig(ref EventHubConfigInfo eventHubSettings)
        {
            eventHubSettings.eventHubNamespace = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.eventHubNamespace"];
            eventHubSettings.eventHubPath = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.eventHubPath"];
            eventHubSettings.SendKeyName = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.SendKeyName"];
            eventHubSettings.SendKeyValue = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.SendKeyValue"];
            eventHubSettings.consumerGroup = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.consumerGroup"];
            eventHubSettings.eventHubConnectionString = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.eventHubConnectionString"];
            eventHubSettings.storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["BandontheRun.storageConnectionString"];
            eventHubSettings.iotHubConnectionString = System.Configuration.ConfigurationManager.AppSettings["iotHubConnectionString"]
 
        }
    }
}
