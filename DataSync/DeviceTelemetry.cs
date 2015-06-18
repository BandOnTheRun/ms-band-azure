using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync
{
    public class DeviceTelemetry
    {
        public DeviceTelemetry()
        {
        }
        public string DeviceId { get; set; }

        public int HeartRate { get; set; }
        public double SkinTemp { get; set; }
        public UVIndexLevel UVIndex { get; set; }

        public string Timestamp { get; set; }
    }
}
