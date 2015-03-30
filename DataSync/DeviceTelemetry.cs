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
            Data = new BandData();
        }
        public string DeviceId { get; set; }
        public BandData Data { get; set; }
    }
}
