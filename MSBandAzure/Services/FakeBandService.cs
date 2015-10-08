using System.Collections.Generic;
using System.Threading.Tasks;
using MSBandAzure.Model;
using Microsoft.Band;
using Autofac;
using MSBandAzure.Services.Fakes;

namespace MSBandAzure.Services
{
    public class FakeBandService : IBandService
    {
        private IComponentContext _container;

        public FakeBandService(IComponentContext container)
        {
            _container = container;
        }

        public async Task<IBandClient> ConnectBandAsync(IBandInfo band)
        {
            await Task.Delay(1000);
            return new FakeBandClient();
        }

        /// <summary>
        /// Create a Fake band object using the dependency injection container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private static Band CreateBand(IComponentContext container, string name)
        {
            var bandInfo = container.Resolve<IBandInfo>(
                new TypedParameter(typeof(BandConnectionType), BandConnectionType.Bluetooth),
                new NamedParameter("name", name),
                new TypedParameter(typeof(IComponentContext), container));

            var band = container.Resolve<Band>(
                new TypedParameter(typeof(IBandInfo), bandInfo));

            return band;
        }

        public async Task<IEnumerable<Band>> GetPairedBands()
        {
            await Task.Delay(1000);

            // TODO: Investigated using AutoPoco but currently has no uwp nuget package - revisit
            return new List<Band>
            {
                CreateBand(_container, "Fake Band 1"),
                CreateBand(_container, "Fake Band 2"),
                CreateBand(_container, "Fake Band 3"),
                CreateBand(_container, "Fake Band 4"),
                CreateBand(_container, "Fake Band 5"),
            };
        }
    }
}
