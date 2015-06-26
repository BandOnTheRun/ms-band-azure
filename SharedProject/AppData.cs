using Caliburn.Micro;
using DataSync;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject
{
    public static class AppData
    {
        static AppData()
        {
            Events = new EventAggregator();
        }
        private static DeviceTelemetry _data = new DeviceTelemetry { DeviceId = "dxband" };

        public static DeviceTelemetry Data { get { return _data; } }
        private static ITelemetry _telemetry = new EventHubsTelemetry();
        public static ITelemetry Telemetry { get { return _telemetry; } }
        public static IEventAggregator Events { get; set; }
    }
}
