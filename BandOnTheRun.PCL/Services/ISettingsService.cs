using System.Collections.Generic;

namespace BandOnTheRun.PCL.Services
{
    public interface ISettingsService
    {
        Dictionary<string, object> Values { get; }
    }
}
