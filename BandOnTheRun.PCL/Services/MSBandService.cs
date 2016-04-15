using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSBandAzure.Model;
using Microsoft.Band;
using System;
using System.Reflection;
using Autofac;
using Microsoft.Band.Portable;

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
            var bandClient = await BandClientManager.Instance.ConnectAsync((BandDeviceInfo)band);

            // Note. The following code is a workaround for a bug in the Band SDK;
            // see the following link 
            // http://stackoverflow.com/questions/30611731/microsoft-band-sdk-sensors-windows-sample-exception
            Type.GetType("Microsoft.Band.BandClient, Microsoft.Band")
                        .GetRuntimeFields()
                        .First(field => field.Name == "currentAppId")
                        .SetValue(bandClient, Guid.NewGuid());

            return (IBandClient)bandClient;
        }

        public async Task<IEnumerable<Band>> GetPairedBands()
        {
            var pairedBands = await BandClientManager.Instance.GetPairedBandsAsync();

            // Convert DeviceBandInfo ojects to IBandInfo..
            var bands = pairedBands.Select(d => new DeviceInfoWrapper(d));
            return bands.Select(i => _container.Resolve<Band>(new TypedParameter(typeof(IBandInfo), i))).ToList();
        }
    }

    internal class DeviceInfoWrapper : IBandInfo
    {
        BandDeviceInfo _dev;
        public DeviceInfoWrapper(BandDeviceInfo dev)
        {
            _dev = dev;
        }

        public BandConnectionType ConnectionType
        {
            get
            {
                return BandConnectionType.Bluetooth;
            }
        }

        public string Name
        {
            get
            {
                return _dev.Name;
            }
        }
    }
}
