using Caliburn.Micro;
using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace DataSync.ViewModels
{
    public class MainViewModel : ViewModelBase, 
                                 IHandle<BandViewModel>,
                                 IHandle<BusyProcessing>
    {
        Lazy<ICommand> _enumerateBandsCmd;
        public ICommand EnumerateBandsCmd { get { return _enumerateBandsCmd.Value; } }

        private List<BandViewModel> _bands;

        public List<BandViewModel> Bands
        {
            get { return _bands; }
            set { SetProperty(ref _bands, value); }
        }

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        private string _loadingText;

        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        Lazy<ICommand> _connectCmd;
        public ICommand ConnectCmd { get { return _connectCmd.Value; } }

        private BandViewModel _connectedBand;

        public BandViewModel ConnectedBand
        {
            get { return _connectedBand; }
            set { SetProperty(ref _connectedBand, value); }
        }

        public MainViewModel()
        {
            App.Events.Subscribe(this);

            _enumerateBandsCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(EnumerateBands, CanEnumerateBands);
            });
            _connectCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Connect, CanConnect);
            });

            EnumerateBandsCmd.Execute(null);
        }

        private bool CanConnect(object arg)
        {
            return ConnectedBand == null && IsBusy == false;
        }

        private async Task<object> Connect(object arg)
        {
            IsBusy = true;
            LoadingText = "Connecting...";
            var item = (ItemClickEventArgs)arg;
            var vm = (BandViewModel)item.ClickedItem;
            await vm.Connect(null);
            IsBusy = false;
            return arg;
        }

        private bool CanEnumerateBands(object obj)
        {
            return true;
        }

        private async Task<object> EnumerateBands(object obj)
        {
            IsBusy = true;
            LoadingText = "Enumerating paired bands...";
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            Bands = new List<BandViewModel>(pairedBands.Select(b => new BandViewModel(b)));
            IsBusy = false;
            return Bands;
        }

        public void Handle(BandViewModel message)
        {
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ConnectedBand = message;
                });
        }

        public void Handle(BusyProcessing message)
        {
            IsBusy = message.IsBusy;
            LoadingText = message.BusyText;
        }
    }
}
