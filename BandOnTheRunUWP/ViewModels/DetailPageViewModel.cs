using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Connect().ContinueWith(t =>
            {

            });
        }

        private async Task Connect()
        {
            if (App.CurrentBand != null)
            {
                if (App.CurrentBand.Connected == false)
                {
                    await App.CurrentBand.Connect(null);
                    await App.CurrentBand.StartSensors();
                }
            }
        }
    }
}
