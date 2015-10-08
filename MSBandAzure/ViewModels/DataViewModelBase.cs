using Microsoft.Band;
using MSBandAzure.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSBandAzure.ViewModels
{
    public abstract class DataViewModelBase : ViewModelBase
    {
        public DataViewModelBase(string name, IBandClient bandClient)
        {
            _bandClient = bandClient;

            _name = name;
            _dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            _startCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Start, CanStart);
            });
            _stopCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Stop, CanStop);
            });
        }

        protected bool _started;
        public bool Started { get { return _started; } }

        protected virtual bool CanStop(object arg) { return true; }
        protected abstract Task<object> Stop(object arg);

        protected virtual bool CanStart(object arg) { return true; }
        protected abstract Task<object> Start(object arg);

        Lazy<ICommand> _startCmd;
        public ICommand StartCmd { get { return _startCmd.Value; } }

        Lazy<ICommand> _stopCmd;
        public ICommand StopCmd { get { return _stopCmd.Value; } }

        protected Windows.UI.Core.CoreDispatcher _dispatcher;

        private string _name;
        public string Name { get { return _name; } }

        private string _timeStamp;

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { Set(ref _timeStamp, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }

        protected IBandClient _bandClient;
    }
}
