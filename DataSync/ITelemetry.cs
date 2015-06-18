using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataSync
{
    public interface ITelemetry
    {
        Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry);
    }
}
