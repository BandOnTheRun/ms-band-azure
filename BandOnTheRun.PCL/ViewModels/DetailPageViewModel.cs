using MSBandAzure.Mvvm;
using System.Threading.Tasks;

namespace MSBandAzure.ViewModels
{
    public class DetailPageViewModel : MSBandAzure.Mvvm.ViewModelBase
    {
        public BandViewModel BandViewModel { get; set; }

        public DetailPageViewModel()
        {
            BandViewModel = VMLocator.Instance.CurrentBand;
            Connect().ContinueWith(t =>
            {

            });
        }

        private async Task Connect()
        {
            if (VMLocator.Instance.CurrentBand != null)
            {
                if (VMLocator.Instance.CurrentBand.Connected == false)
                {
                    await VMLocator.Instance.CurrentBand.Connect(null);
                    await VMLocator.Instance.CurrentBand.StartSensors();
                }
            }
        }
    }
}
