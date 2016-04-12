using Autofac;
using Caliburn.Micro;
using MSBandAzure.Model;
using MSBandAzure.Mvvm;
using MSBandAzure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSBandAzure.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        ObservableCollection<BandViewModel> _bands;
        public ObservableCollection<BandViewModel> Bands
        {
            get { return _bands; }
            set { SetProperty(ref _bands, value); }
        }

        private readonly IBandService _bandService;
        private readonly IComponentContext _container;
        private readonly IEventAggregator _events;
        private readonly INavigationService _navigation;

        public MainPageViewModel(IBandService bandService, IEventAggregator events, INavigationService naviagtionService,
            IComponentContext container)
        {
            _container = container;
            _navigation = naviagtionService;
            _bandService = bandService;
            _events = events;
            _enumerateBandsCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(EnumerateBands, CanEnumerateBands);
            });

            EnumerateBandsCmd.Execute(null);
        }

        private string _Value = string.Empty;
        public string Value { get { return _Value; } set { SetProperty(ref _Value, value); } }

        public void GotoDetailsPage(object item)
        {
            VMLocator.Instance.CurrentBand = item as BandViewModel;
            _navigation.Navigate("Views.DetailPage", Value);
        }

        private async Task<object> EnumerateBands(object obj)
        {
            if (Bands != null && Bands.Count > 0)
                Bands.Clear();

            IsBusy = true;

            var bands = await _bandService.GetPairedBands();
            Bands = new ObservableCollection<BandViewModel>(bands.Select(b => 
                _container.Resolve<BandViewModel>(new TypedParameter(typeof(Band), b))));

            // If we know about the band then try to auto-connect
            if (VMLocator.Instance.SettingsViewModel.AutoConnect == true)
            {
                AutoConnectBandsAsync(Bands);
            }

            return Bands;
        }

        private async Task AutoConnectBandsAsync(IEnumerable<BandViewModel> bands)
        {
            foreach (var band in bands)
            {
                if (band.ConnectCmd.CanExecute(null) == true)
                    band.ConnectCmd.ExecuteAsync(null);
            }
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
            set { SetProperty(ref _isBusy, value); }
        }
    }
}
