using Autofac;
using Caliburn.Micro;
using Microsoft.Band;
using MSBandAzure.Model;
using MSBandAzure.Services;
using MSBandAzure.Services.Fakes;
using MSBandAzure.ViewModels;

namespace MSBandAzure.Mvvm
{
    public class VMLocator
    {
        private IContainer _container;

        public VMLocator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainPageViewModel>().SingleInstance();
            builder.RegisterType<DetailPageViewModel>().InstancePerDependency();

            builder.RegisterType<HeartRateViewModel>().InstancePerDependency();
            builder.RegisterType<SkinTempViewModel>().InstancePerDependency();
            builder.RegisterType<UVViewModel>().InstancePerDependency();

#if DEBUG
            builder.RegisterType<FakeBandService>().As<IBandService>().SingleInstance();
            builder.RegisterType<FakeBandInfo>().As<IBandInfo>().InstancePerDependency();
#else
            builder.RegisterType<MSBandService>( ).As<IBandService>().SingleInstance();
#endif
            builder.RegisterType<Band>().InstancePerDependency();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            _container = builder.Build();
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return _container.Resolve<MainPageViewModel>();
            }
        }

        public DetailPageViewModel DetailPageViewModel
        {
            get
            {
                return _container.Resolve<DetailPageViewModel>();
            }
        }
    }
}
