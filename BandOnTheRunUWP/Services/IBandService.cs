using Microsoft.Band;
using MSBandAzure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSBandAzure.Services
{
    public interface IBandService
    {
        Task<IEnumerable<Band>> GetPairedBands();

        Task<IBandClient> ConnectBandAsync(IBandInfo band);
    }
}
