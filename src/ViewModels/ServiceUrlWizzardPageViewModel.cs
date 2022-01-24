using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ServiceUrlWizzardPageViewModel : ViewModelBase
    {
        private SettingsService _settingsService => SettingsService.Instance;
        private IDialogService m_dialogSService;
        private IAuthenticationService m_authenticationHandler;
        private RelayCommand m_saveHostCommand;
        private string m_strServiceUrl;

        public string ServiceUrl
        {
            get
            {
                return this.m_strServiceUrl;
            }
            set
            {
                this.m_strServiceUrl = value;
                this.RaisePropertyChanged("ServiceUrl");
                this.SaveHostCommand.RaiseCanExecuteChanged();
            }
        }

        public ServiceUrlWizzardPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //_settingsService = SettingsService.Instance;
                //m_dialogSService = DialogService.Instance;
                //m_authenticationHandler = AuthenticationService.Instance;
                //ServiceUrl = m_settingsService.GetServiceUrl.ServiceUrl;
            }
        }
        public RelayCommand SaveHostCommand => m_saveHostCommand ?? (m_saveHostCommand = new RelayCommand(SaveUrl));

        public async void SaveUrl()
        {
            var serviceUrl = ServiceUrl;
            try
            {
                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    UriBuilder uriBuilder = new UriBuilder(serviceUrl);
                    serviceUrl = uriBuilder.Uri.AbsoluteUri;
                }

                await DataService.IsHostAccessible(serviceUrl);
                _settingsService.ServiceUrl = serviceUrl;
                try
                {
                    User user = await m_authenticationHandler.VerifyUserAuthenticationAsync().ConfigureAwait(true);
                    if (user == null)
                    {
                        _settingsService.IsFullScreen = true;
                        await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                    }
                    else
                    {
                        _settingsService.IsFullScreen = false;
                        await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    _settingsService.IsFullScreen = true;
                    await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                }
            }
            catch (Exception)
            {
                await m_dialogSService.ShowMessageDialogAsync(
                    ResourceService.GetString("ServiceUrlNotAvailableExceptionMessage", "The address of your webserver was entered incorrectly or the webserver is not available."),
                    ResourceService.GetString("ExceptionMessageDialogHeader", "Error"));
            }
        }
        }
}
