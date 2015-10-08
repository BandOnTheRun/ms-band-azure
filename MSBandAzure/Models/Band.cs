using Microsoft.Band;
using System.Threading.Tasks;
using MSBandAzure.Services;
using MSBandAzure.ViewModels;
using Autofac;

namespace MSBandAzure.Model
{
    public class Band
    {
        private readonly IBandInfo _info;
        private IBandClient _bandClient;
        private readonly IBandService _bandService;
        private readonly IComponentContext _container;

        public IBandClient Client {  get { return _bandClient; } }

        public Band(IBandInfo info, IBandService bandService, IComponentContext container)
        {
            _container = container;
            _info = info;
            _bandService = bandService;
        }
        public string Name { get { return _info.Name; } }

        public bool Connected { get; set; }

        public async Task Connect()
        {
            _bandClient = await _bandService.ConnectBandAsync(_info);
            Connected = true;
        }

        internal DataViewModelBase CreateSensorViewModel<T>() where T : DataViewModelBase
        {
            return _container.Resolve<T>(new TypedParameter(typeof(IBandClient), _bandClient));
        }
    }
}