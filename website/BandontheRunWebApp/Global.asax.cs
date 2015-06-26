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
        public string ehConnectionString { get; set; }
        public string storageConnectionString { get; set; } 
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



        private void CreateEventProcessorHostClient(ref EventHubConfigInfo eventHubSettings)
        {
          try
            {
                eventHubSettings.processorHost = new EventProcessorHost(
                        this.Server.MachineName,
                        eventHubSettings.eventHubPath,
                        eventHubSettings.consumerGroup.ToLowerInvariant(),
                        eventHubSettings.ehConnectionString,
                        eventHubSettings.storageConnectionString);


                // could now set eventHubSettings.processorHostOptions 


                // wire up our class to handle events from event hub
                eventHubSettings.processorHost.RegisterEventProcessorAsync<WebsiteEventProcessor>().Wait();
            }
            catch (Exception e)
            {
                Debug.Print("Error happened while trying to connect Event Hub - {0}", e.Message);
            }

        }



        private void GetEventHubConsumerGroupConfig(ref EventHubConfigInfo eventHubSettings)
        {
            eventHubSettings.eventHubNamespace = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.eventHubNamespace");
            eventHubSettings.eventHubPath = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.eventHubPath");
            eventHubSettings.SendKeyName = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SendKeyName");
            eventHubSettings.SendKeyValue = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.SendKeyValue");
            eventHubSettings.consumerGroup = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.consumerGroup");
            eventHubSettings.ehConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ehConnectionString");
            eventHubSettings.storageConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.storageConnectionString");
        }
    }
}
