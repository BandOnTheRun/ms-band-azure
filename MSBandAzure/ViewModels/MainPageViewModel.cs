using Caliburn.Micro;
using MSBandAzure.Mvvm;
using MSBandAzure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace MSBandAzure.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        ObservableCollection<BandViewModel> _bands;
        public ObservableCollection<BandViewModel> Bands
        {
            get { return _bands; }
            set { Set(ref _bands, value); }
        }

        private readonly IBandService _bandService;
        private readonly IEventAggregator _events;

        public MainPageViewModel(IBandService bandService, IEventAggregator events)
        {
            _bandService = bandService;
            _events = events;
            _enumerateBandsCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(EnumerateBands, CanEnumerateBands);
            });

            EnumerateBandsCmd.Execute(null);

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime data
                Value = "Designtime value";
                return;
            }
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            {
                // use cache value(s)
                if (state.ContainsKey(nameof(Value))) Value = state[nameof(Value)]?.ToString();
                // clear any cache
                state.Clear();
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                // persist into cache
                state[nameof(Value)] = Value;
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(NavigatingEventArgs args)
        {
            base.OnNavigatingFrom(args);
        }

        private string _Value = string.Empty;
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public void GotoDetailsPage(object item)
        {
            App.CurrentBand = item as BandViewModel;
            this.NavigationService.Navigate(typeof(Views.DetailPage), this.Value);
        }

        private async Task<object> EnumerateBands(object obj)
        {
            if (Bands != null && Bands.Count > 0)
                Bands.Clear();

            IsBusy = true;

            var bands = await _bandService.GetPairedBands();
            Bands = new ObservableCollection<BandViewModel>(bands.Select(b => new BandViewModel(b)));
            
            //_events.Publish();

            //App.Events.Publish(new BusyProcessing { IsBusy = true, BusyText = "Enumerating bands..." });
            //IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            //Bands = new List<BandViewModel>(pairedBands.Select(b => new BandViewModel(b)));
            //IsBusy = false;
            //App.Events.Publish(new BusyProcessing { IsBusy = false, BusyText = "" });

            return Bands;
        }
        private bool CanEnumerateBands(object obj)
        {
            return true;
        }

        Lazy<ICommand> _enumerateBandsCmd;
        public ICommand EnumerateBandsCmd { get { return _enumerateBandsCmd.Value; } }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }
    }
}
