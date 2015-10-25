﻿using Autofac;
using Caliburn.Micro;
using Microsoft.Band;
using MSBandAzure.Model;
using MSBandAzure.Services;
using MSBandAzure.Services.Fakes;
using MSBandAzure.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MSBandAzure.Mvvm
{
    public class VMLocator
    {
        private IContainer _container;
        private Dictionary<Band, BandViewModel> _bandCache = new Dictionary<Band, BandViewModel>();

        public VMLocator(INavigationService nav)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(nav);

            builder.RegisterType<MainPageViewModel>().SingleInstance();

            // Factory to retrieve band view model from a cache and thus associate Band model objects
            // with BandViewModel objects - sure there must be a way to do this in AutoFac but haven't
            // found it yet.
            builder.Register((c, p) =>
            {
                var p1 = (TypedParameter)p.First();
                Band parameter = (Band)p1.Value;
                BandViewModel outVal = null;
                if (_bandCache.TryGetValue(parameter, out outVal))
                    return outVal;

                outVal = new BandViewModel(parameter);
                _bandCache[parameter] = outVal;
                return outVal;
            });

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
            builder.RegisterType<EventHubsTelemetry>().As<ITelemetry>().SingleInstance();
            builder.RegisterType<Band>().InstancePerDependency();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            _container = builder.Build();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
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
