using System.Threading.Tasks;
using Windows.Web.Http;

namespace DataSync
{
    public interface ITelemetry
    {
        Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry);
    }
}
