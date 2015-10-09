namespace BandOnTheRunEventHubMonitor
{
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.DataContracts;
  using Microsoft.ServiceBus;
  using System;
  using System.Configuration;

  class Program
  {
    static void Main()
    {
      Console.WriteLine("Hello, this is a trace");

      try
      {
        var eventHubString =
          ConfigurationManager.ConnectionStrings["EventHubConnection"];

        var eventHubName =
          ConfigurationManager.ConnectionStrings["EventHubName"];

        var appInsightsKey =
          ConfigurationManager.ConnectionStrings["AppInsightsKey"];

        var nsm = NamespaceManager.CreateFromConnectionString(eventHubString.ConnectionString);

        var hub = nsm.GetEventHub(eventHubName.ConnectionString);

        TelemetryClient appInsights = new TelemetryClient();
        appInsights.InstrumentationKey = appInsightsKey.ConnectionString;
        var telemetryData = new EventTelemetry()
        {
          Name = "Event Hub Heartbeat",
        };
        telemetryData.Properties.Add("Status", hub.Status.ToString());
        appInsights.TrackEvent(telemetryData);
        appInsights.Flush();

        Console.Write($"Service status is at {hub.Status}");
      }
      catch
      {
        Console.WriteLine("Catch handler");
      }

      Console.WriteLine("We're done");
    }
  }
}
