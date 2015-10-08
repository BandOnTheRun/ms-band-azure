using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace MSBandAzure.ViewModels
{
    public class DetailPageViewModel : MSBandAzure.Mvvm.ViewModelBase
    {
        public BandViewModel BandViewModel { get; set; }

        public DetailPageViewModel()
        {
            BandViewModel = App.CurrentBand;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime data
                //this.Value = "Designtime value";
                return;
            }
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            {
                // use cache value(s)
                //if (state.ContainsKey(nameof(Value))) Value = state[nameof(Value)]?.ToString();
                // clear any cache
                state.Clear();
            }
            else
            {
                // use navigation parameter
                //Value = parameter?.ToString();
            }

            if (App.CurrentBand != null)
            {
                if (App.CurrentBand.Connected == false)
                {
                    await App.CurrentBand.Connect(null);
                    await App.CurrentBand.StartSensors();
                }
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                //// persist into cache
                //state[nameof(Value)] = Value;
            }
            return base.OnNavigatedFromAsync(state, suspending);
        }

        public override void OnNavigatingFrom(NavigatingEventArgs args)
        {
            args.Cancel = false;
        }
    }
}
