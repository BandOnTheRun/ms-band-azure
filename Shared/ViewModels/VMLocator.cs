using Autofac;

namespace DataSync.ViewModels
{
    public class VMLocator
    {
        private readonly IContainer _container;

        public VMLocator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DevicesViewModel>().SingleInstance();
            _container = builder.Build();
        }

        public DevicesViewModel DevicesViewModel
        {
            get
            {
                return _container.Resolve<DevicesViewModel>();
            }
        }
    }
}
