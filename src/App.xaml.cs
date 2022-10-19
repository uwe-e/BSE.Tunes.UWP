using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.ViewModels;
using CommonServiceLocator;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp
{
    [Bindable]
    sealed partial class App : Application
    {
        private readonly Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }
        private SettingsService m_settingsService;

        public App()
        {
            InitializeComponent();

            UnhandledException += OnAppUnhandledException;

            // some settings must be set in app.constructor
            m_settingsService = SettingsService.Instance;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);

                var navigationService = ViewModels.ViewModelLocator.Current.NavigationService;
                navigationService.Frame.Style = Resources["RootFrameStyle"] as Style;
                
                IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
                try
                {
                    var isAccessible = await dataService.IsHostAccessible().ConfigureAwait(true);
                    if (isAccessible)
                    {
                        try
                        {
                            IAuthenticationService authenticationService = ViewModelLocator.Current.AuthenticationService;
                            User user = await authenticationService.VerifyUserAuthenticationAsync().ConfigureAwait(true);
                            if (user == null)
                            {
                                await navigationService.NavigateAsync(typeof(Views.SignInWizzardPage), navitageFullscreen: true);
                            }
                            else
                            {
                                await navigationService.NavigateAsync(typeof(Views.MainPage));
                            }
                            
                        }
                        catch(Exception)
                        {
                            throw;
                        }
                    }
                }
                catch (Exception)
                {
                    await navigationService.NavigateAsync(typeof(Views.ServiceUrlWizzardPage), navitageFullscreen:true);
                }
            }

            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            //IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
            //try
            //{
            //    var t = Task.Run(() =>
            //    {
            //        return dataService.IsHostAccessible().ConfigureAwait(true);
            //    });

            //    var h = t;

            //}
            //catch (Exception ex)
            //{

            //}
            //return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
            //return new ActivationService(this, typeof(Views.ServiceUrlWizzardPage));
            return new ActivationService(this, new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }
    }
}

