using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using MSBandAzure.Mvvm;
using MSBandAzure.ViewModels;
using Microsoft.ApplicationInsights;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Core;
using MSBandAzure.Views;
using MSBandAzure.Services;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Newtonsoft.Json;

namespace MSBandAzure
{
#if TEMPLATE10
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
            WindowsAppInitializer.InitializeAsync();

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

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(Views.MainPage), null);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            //Window.Current.Content = new Views.Shell(nav);
            //Window.Current.Content = new Views.MainPage();

            return Task.FromResult<object>(null);
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await Task.Delay(500);
            NavigationService.Navigate(typeof(Views.MainPage));
        }
    }
#else
    sealed partial class App : Application, INavigationService
    {
        public static VMLocator Locator;
        public static BandViewModel CurrentBand = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Locator = new VMLocator(this);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private Frame _rootFrame;
        public static dynamic Settings;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

            //#if DEBUG
            //            if (System.Diagnostics.Debugger.IsAttached)
            //            {
            //                this.DebugSettings.EnableFrameRateCounter = true;
            //            }
            //#endif

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///config.json"));
            //var str = await file.OpenReadAsync();
            var str = await FileIO.ReadTextAsync(file);
            Settings = JsonConvert.DeserializeObject(str);

            _rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (_rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                _rootFrame = new Frame();

                _rootFrame.Navigated += OnNavigated;
                _rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = _rootFrame;

                // listen for backbutton requests
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                UpdateBackButtonVisibility();
            }

            if (_rootFrame.Content == null)
            {
                // navigate to the master page providing the navigation structure
                //_rootFrame.Navigate(typeof(MainPage), null);
                _rootFrame.Navigate(typeof(Shell), null);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            try
            {
                // Install the main VCD. Since there's no simple way to test that the VCD has been imported, or that it's your most recent
                // version, it's not unreasonable to do this upon app load.
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"BandOnTheRunCommands.xml");

                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }
        }

        private void UpdateBackButtonVisibility()
        {
            var visibility = AppViewBackButtonVisibility.Collapsed;
            var frame = (Frame)Window.Current.Content;
            if (frame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public void Navigate(Type target, object param = null)
        {
            _rootFrame.Navigate(target, param);
        }

        //protected override void OnActivated(IActivatedEventArgs args)
        //{
        //    base.OnActivated(args);


        //    _rootFrame = Window.Current.Content as Frame;

        //    // Do not repeat app initialization when the Window already has content,
        //    // just ensure that the window is active
        //    if (_rootFrame == null)
        //    {
        //        // Create a Frame to act as the navigation context and navigate to the first page
        //        _rootFrame = new Frame();

        //        _rootFrame.Navigated += OnNavigated;
        //        _rootFrame.NavigationFailed += OnNavigationFailed;

        //        // Place the frame in the current Window
        //        Window.Current.Content = _rootFrame;

        //        // listen for backbutton requests
        //        SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        //        UpdateBackButtonVisibility();
        //    }

        //    if (_rootFrame.Content == null)
        //    {
        //        // navigate to the master page providing the navigation structure
        //        _rootFrame.Navigate(typeof(MainPage), null);
        //    }
        //    // Ensure the current window is active
        //    Window.Current.Activate();
        //}
    }

#endif

}
