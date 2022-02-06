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
        private IDialogService _dialogSService => DialogService.Instance;
        private IAuthenticationService _authenticationHandler => ViewModelLocator.Current.AuthenticationService;
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
                    User user = await _authenticationHandler.VerifyUserAuthenticationAsync().ConfigureAwait(true);
                    if (user == null)
                    {
                        await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage), navitageFullscreen: true);
                    }
                    else
                    {
                        await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage), navitageFullscreen: true);
                }
            }
            catch (Exception)
            {
                await _dialogSService.ShowMessageDialogAsync(
                    ResourceService.GetString("ServiceUrlNotAvailableExceptionMessage", "The address of your webserver was entered incorrectly or the webserver is not available."),
                    ResourceService.GetString("ExceptionMessageDialogHeader", "Error"));
            }
        }
        }
}
