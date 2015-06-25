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
    public class DevicesViewModel : ViewModelBase
    {
        public DevicesViewModel()
        {
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

        Lazy<ICommand> _connectCmd;
        public ICommand ConnectCmd { get { return _connectCmd.Value; } }

        private bool CanConnect(object arg)
        {
            return IsBusy == false;
        }

        private async Task<object> Connect(object arg)
        {
            App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Connecting..." });
            IsBusy = true;
            ((AsyncDelegateCommand<object>)(ConnectCmd)).RaiseCanExecuteChanged();

            LoadingText = "Connecting...";
            var item = (ItemClickEventArgs)arg;
            var vm = (BandViewModel)item.ClickedItem;
            await vm.Connect(null);
            IsBusy = false;
            App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
            return arg;
        }

        public async Task ConnectToBandAsync(string bandName)
        {
            var bands = await EnumerateBands(null);
            var bandList = bands as IEnumerable<BandViewModel>;
            if (bandList == null || bandList.Count() < 1)
                return;
            var band = bandList.SingleOrDefault(b => b.Info.Name == bandName);
            if (band == null)
                return;
            await band.ConnectCmd.ExecuteAsync(null);
        }

        private bool CanEnumerateBands(object obj)
        {
            return true;
        }

        private string _loadingText;

        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        private async Task<object> EnumerateBands(object obj)
        {
            if (Bands != null && Bands.Count > 0)
                Bands.Clear();

            IsBusy = true;
            App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Enumerating bands..." });
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            Bands = new List<BandViewModel>(pairedBands.Select(b => new BandViewModel(b)));
            IsBusy = false;
            App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });
            return Bands;
        }

        Lazy<ICommand> _enumerateBandsCmd;
        public ICommand EnumerateBandsCmd { get { return _enumerateBandsCmd.Value; } }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private List<BandViewModel> _bands;

        public List<BandViewModel> Bands
        {
            get { return _bands; }
            set { SetProperty(ref _bands, value); }
        }
    }
}
