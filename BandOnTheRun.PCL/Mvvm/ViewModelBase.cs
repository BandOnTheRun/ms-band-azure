using BandOnTheRun.PCL.Services;
using PCLCommandBase;

namespace MSBandAzure.Mvvm
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-MVVM
    public abstract class ViewModelBase : BindableBase /*: Template10.Mvvm.ViewModelBase*/
    {
        protected IDispatcher _dispatcher;

        public ViewModelBase()
        {
            _dispatcher = VMLocator.Instance.Resolve<IDispatcher>();
        }
        // the only thing that matters here is Template10.Services.NavigationService.INavagable
    }
}