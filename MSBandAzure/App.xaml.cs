using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using MSBandAzure.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using MSBandAzure.Mvvm;
using MSBandAzure.ViewModels;
using Microsoft.ApplicationInsights;


namespace MSBandAzure
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Bootstrapper
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Cache
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-BackButton
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplashScreen
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-NavigationService

    sealed partial class App : Template10.Common.BootStrapper
    {
        public static VMLocator Locator = new VMLocator();
        public static BandViewModel CurrentBand = null;

        public App()
        {
            // initialize application insights
            WindowsAppInitializer.InitializeAsync("44aee2b2-7335-4a03-9f91-c817f22dcc19");

            // then init the components for the app
            InitializeComponent();

            CacheMaxDuration = TimeSpan.FromDays(2);
            ShowShellBackButton = SettingsService.Instance.UseShellBackButton;
            SplashFactory = (e) => new Views.Splash(e);
        }

        // runs even if restored from state
        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            return Task.FromResult<object>(null);
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await Task.Delay(5000);
            NavigationService.Navigate(typeof(Views.MainPage));
        }
    }
}
