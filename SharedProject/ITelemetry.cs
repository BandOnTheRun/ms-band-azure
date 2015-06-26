using System.Net.Http;
using System.Threading.Tasks;

namespace DataSync
{
    public interface ITelemetry
    {
        Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry);
    }
}
