using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ServiceUrlWizzardPageViewModel : ViewModelBase
    {
        private SettingsService _settingsService => SettingsService.Instance;
        private IDialogService _dialogSService => DialogService.Instance;
        private IAuthenticationService _authenticationHandler => ViewModelLocator.Current.AuthenticationService;
        private RelayCommand _saveHostCommand;
        private ICommand _saveServiceUrlCommand;
        private string _strServiceUrl;
        
        public RelayCommand SaveHostCommand => _saveHostCommand ?? (_saveHostCommand = new RelayCommand(SaveUrl));

        public ICommand SaveServiceUrlCommand => _saveServiceUrlCommand ?? (_saveServiceUrlCommand = new RelayCommand<string>(SaveServiceUrl));

        public string ServiceUrl
        {
            get
            {
                return this._strServiceUrl;
            }
            set
            {
                this._strServiceUrl = value;
                this.RaisePropertyChanged("ServiceUrl");
                this.SaveHostCommand.RaiseCanExecuteChanged();
            }
        }

        public ServiceUrlWizzardPageViewModel()
        {
        }

        public void SaveUrl()
        {
            SaveServiceUrl(ServiceUrl);
        }

        private async void SaveServiceUrl(string serviceUrl)
        {
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
