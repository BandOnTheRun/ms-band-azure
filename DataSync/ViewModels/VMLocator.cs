using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace DataSync.ViewModels
{
    public class VMLocator
    {
        public TinyIoCContainer _container = new TinyIoCContainer();

        public VMLocator()
        {
            _container.Register<DevicesViewModel>().AsSingleton();
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
