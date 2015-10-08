using System.Threading.Tasks;
using Windows.Web.Http;
using MSBandAzure.Models;

namespace MSBandAzure.Services
{
    public interface ITelemetry
    {
        Task<HttpResponseMessage> PostTelemetryAsync(DeviceTelemetry deviceTelemetry);
    }
}
