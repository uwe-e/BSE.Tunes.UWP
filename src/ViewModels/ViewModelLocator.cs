using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register<ShellViewModel>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IResourceService, ResourceService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IPlayerService, PlayerService>();
            SimpleIoc.Default.Register<IPlayerManager, PlayerManager>();
            SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();
            SimpleIoc.Default.Register<ICacheableBitmapService, CacheableBitmapService>();

            //SimpleIoc.Default.Register<SettingsMainPageViewModel>();
        }

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        public IAuthenticationService AuthenticationService => SimpleIoc.Default.GetInstance<IAuthenticationService>(); 
        
        public ShellViewModel ShellViewModel => SimpleIoc.Default.GetInstance<ShellViewModel>();


        //public SettingsMainPageViewModel SettingsHostPageViewModel
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<SettingsMainPageViewModel>();
        //    }
        //}

        //private MainPageViewModel m_MainPageViewModel;
        //public MainPageViewModel MainPageViewModel => m_MainPageViewModel ?? (m_MainPageViewModel = new MainPageViewModel());

        //private SettingsHostPageViewModel m_settingsHostPageViewModel;
        //public SettingsHostPageViewModel SettingsHostItemViewModel => m_settingsHostPageViewModel ?? (m_settingsHostPageViewModel = new SettingsHostPageViewModel());

        //private ServiceUrlWizzardPageViewModel m_HostSettingsPageViewModel;
        //public ServiceUrlWizzardPageViewModel HostSettingsPageViewModel => m_HostSettingsPageViewModel ?? (m_HostSettingsPageViewModel = new ServiceUrlWizzardPageViewModel());

        private PlayerBarUserControlViewModel m_playerBarViewModel;
        public PlayerBarUserControlViewModel PlayerBarViewModel => m_playerBarViewModel ?? (m_playerBarViewModel = new PlayerBarUserControlViewModel());

    }
}
