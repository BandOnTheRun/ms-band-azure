using Caliburn.Micro;
using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        Lazy<ICommand> _connectionsCmd;
        public ICommand ConnectionsCmd { get { return _connectionsCmd.Value; } }

        Lazy<ICommand> _settingsCmd;
        public ICommand SettingsCmd { get { return _settingsCmd.Value; } }

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
            _startStopCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(StartStop, CanStartStop);
            });

            _connectionsCmd = new Lazy<ICommand>(() =>
            {
                return new DelegateCommand(Connections, CanConnections);
            });
            _settingsCmd = new Lazy<ICommand>(() =>
            {
                return new DelegateCommand(Settings, CanSettings);
            });

            EnumerateBandsCmd.Execute(null);
            StartStopIcon = new SymbolIcon(Symbol.Play);
            StartStopLabel = "start";
        }

        private bool CanSettings(object obj)
        {
            return true;
        }

        private void Settings(object obj)
        {
        }

        private bool CanConnections(object obj)
        {
            return true;
        }

        private void Connections(object obj)
        {
        }

        private bool CanStartStop(object arg)
        {
            return ConnectedBand != null;
        }

        private async Task<object> StartStop(object arg)
        {
            // if started, then stop, otherwise if stopped then start..
            if (ConnectedBand == null)
                return null;

            if (Started == false)
            {
                // Could try executing these in parallel - not sure if the band will like it
                foreach (var data in ConnectedBand.SensorData)
                {
                    var asyncCmd = data.StartCmd as IAsyncCommand;
                    if (asyncCmd != null)
                    {
                        try
                        {
                            await asyncCmd.ExecuteAsync(null);
                        }
                        catch (Exception ex)
                        { }
                    }
                }

                // set up state..
                Started = true;
                StartStopLabel = "stop";
                StartStopIcon = new SymbolIcon(Symbol.Stop);
            }
            else
            {
                foreach (var data in ConnectedBand.SensorData)
                {
                    var asyncCmd = data.StopCmd as IAsyncCommand;
                    if (asyncCmd != null)
                    {
                        try
                        {
                            await asyncCmd.ExecuteAsync(null);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                Started = false;
                StartStopLabel = "start";
                StartStopIcon = new SymbolIcon(Symbol.Play);
            }
            return null;
        }

        public bool Started { get; set; }
        private string _startStopLabel;

        public string StartStopLabel
        {
            get { return _startStopLabel; }
            set { SetProperty(ref _startStopLabel, value); }
        }

        private IconElement _startStopIcon;

        public IconElement StartStopIcon
        {
            get { return _startStopIcon; }
            set { SetProperty(ref _startStopIcon, value); }
        }

        Lazy<ICommand> _startStopCmd;
        public ICommand StartStopCmd { get { return _startStopCmd.Value; } }

        private bool CanConnect(object arg)
        {
            return ConnectedBand == null && IsBusy == false;
        }

        private async Task<object> Connect(object arg)
        {
            IsBusy = true;
            ((AsyncDelegateCommand<object>)(ConnectCmd)).RaiseCanExecuteChanged();

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
                    if (ConnectedBand != message)
                    {
                        ConnectedBand = message;
                        ((AsyncDelegateCommand<object>)(StartStopCmd)).RaiseCanExecuteChanged();
                    }
                });

            // Kick off a timer which will post up the telemetry data...
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5)).Subscribe(PostCurrentData);
        }

        // TODO: fix this async void...
        private async void PostCurrentData(long t)
        {
            var resp = await App.Telemetry.PostTelemetryAsync(App.Data);
            if (resp.IsSuccessStatusCode)
            {
                var status = resp.StatusCode;
                var res = await resp.Content.ReadAsStringAsync();
            }

        }

        public void Handle(BusyProcessing message)
        {
            IsBusy = message.IsBusy;
            LoadingText = message.BusyText;
        }
    }
}
