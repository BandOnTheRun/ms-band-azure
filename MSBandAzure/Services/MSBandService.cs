using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSBandAzure.Model;
using Microsoft.Band;
using System;
using System.Reflection;
using Autofac;

namespace MSBandAzure.Services
{
    public class MSBandService : IBandService
    {
        private IComponentContext _container;

        public MSBandService(IComponentContext container)
        {
            _container = container;
        }

        public async Task<IBandClient> ConnectBandAsync(IBandInfo band)
        {
            var bandClient = await BandClientManager.Instance.ConnectAsync(band);

            // Note. The following code is a workaround for a bug in the Band SDK;
            // see the following link 
            // http://stackoverflow.com/questions/30611731/microsoft-band-sdk-sensors-windows-sample-exception
            Type.GetType("Microsoft.Band.BandClient, Microsoft.Band")
                        .GetRuntimeFields()
                        .First(field => field.Name == "currentAppId")
                        .SetValue(bandClient, Guid.NewGuid());

            return bandClient;
        }

        public async Task<IEnumerable<Band>> GetPairedBands()
        {
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            //_container.Resolve<Band>(new TypedParameter(typeof(IBandInfo), );
            return pairedBands.Select(i => _container.Resolve<Band>(new TypedParameter(typeof(IBandInfo), i))).ToList();
            //return pairedBands.Select(i => new Band(i, this)).ToList();
        }
    }
}
