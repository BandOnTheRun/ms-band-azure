using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataSync.ViewModels
{
    public class BandViewModel : ViewModelBase
    {
        private IBandInfo _info;
        public IBandInfo Info { get { return _info; } }
        
        public BandViewModel(IBandInfo info)
        {
            _info = info;
            _connectCmd = new Lazy<ICommand>(() =>
            {
                return new AsyncDelegateCommand<object>(Connect, CanConnect);
            });

        }

        private bool CanConnect(object arg)
        {
            return true;
        }

        public async Task<object> Connect(object arg)
        {
            try
            {
                _bandClient = await BandClientManager.Instance.ConnectAsync(Info);

                InitialiseBandClient(_bandClient);

                ConnectedText = "Connected";
                IsConnected = true;
                App.Events.Publish(this);
                SensorData = new List<BandDataViewModel>
                    {
                        new HeartRateViewModel(_bandClient),
                        new SkinTempViewModel(_bandClient),
                        new UVViewModel(_bandClient),
                    };
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ConnectedText = "";
            }
            return _bandClient;
        }

        private void InitialiseBandClient(IBandClient _bandClient)
        {
            Hr = new HeartRateViewModel(_bandClient);
        }

        public string ConnectionType { get { return Info.ConnectionType.ToString(); } }

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; }
        }

        private IBandClient _bandClient;

        Lazy<ICommand> _connectCmd;
        public ICommand ConnectCmd { get { return _connectCmd.Value; } }

        private string _connectedText;

        public string ConnectedText
        {
            get { return _connectedText; }
            set { SetProperty(ref _connectedText, value); }
        }

        public List<BandDataViewModel> SensorData { get; set; }
         
        private HeartRateViewModel _hr;

        public HeartRateViewModel Hr
        {
            get { return _hr; }
            set { _hr = value; }
        }
    }
}
