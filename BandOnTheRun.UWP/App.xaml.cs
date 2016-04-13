using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using System;
using MSBandAzure.Mvvm;
using MSBandAzure.Services;
using Autofac;
using BandOnTheRun.PCL.Services;
using Windows.Storage;

namespace BandOnTheRun.UWP
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper, INavigationService
    {
        public App()
        {
            VMLocator.CreateInstance(this);

            var builder = new ContainerBuilder();
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();
            builder.RegisterType<SettingsService>().As<ISettingsService>();
            builder.Update(VMLocator.Instance.Container);

            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here

            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        public void Navigate(string target, object param = null)
        {
            Type type = Type.GetType($"BandOnTheRun.UWP.Views.{target}, BandOnTheRun.UWP");
            NavigationService.Navigate(type, param);
        }
    }

    internal class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            var ac = roamingSettings.Values["AutoConnect"];
            _autoConnect = (ac == null || (bool)ac == false) ? false : true;
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }

        private void Current_DataChanged(ApplicationData sender, object args)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            var ac = roamingSettings.Values["AutoConnect"];
            AutoConnect = (ac == null) ? false : true;
        }

        private bool _autoConnect;

        public bool AutoConnect
        {
            get
            {
                return _autoConnect;
            }
            set
            {
                ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
                roamingSettings.Values["AutoConnect"] = value;
            }
        }
    }

    internal class Dispatcher : IDispatcher
    {
        protected Windows.UI.Core.CoreDispatcher _dispatcher;

        public Dispatcher()
        {
            var window = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            if (window != null)
                _dispatcher = window.Dispatcher;
        }

        public Task RunAsync(Action action)
        {
            return _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                new Windows.UI.Core.DispatchedHandler(action)).AsTask();
        }
    }
}

