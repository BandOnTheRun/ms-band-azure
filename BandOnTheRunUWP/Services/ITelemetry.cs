using System.Threading.Tasks;
using Windows.Web.Http;
using MSBandAzure.Models;

namespace MSBandAzure.Services
{
    public interface ITelemetry
    {
        Task RefreshTokenAsync(string deviceId);

        Task PostTelemetryAsync(DeviceTelemetry deviceTelemetry);
    }
}
