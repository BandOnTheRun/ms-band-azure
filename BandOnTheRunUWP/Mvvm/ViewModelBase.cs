using PCLCommandBase;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace MSBandAzure.Mvvm
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-MVVM
    public abstract class ViewModelBase : BindableBase /*: Template10.Mvvm.ViewModelBase*/
    {
        protected Windows.UI.Core.CoreDispatcher _dispatcher;

        public ViewModelBase()
        {
            var window = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            if (window != null)
                _dispatcher = window.Dispatcher;
        }
        // the only thing that matters here is Template10.Services.NavigationService.INavagable
    }
}