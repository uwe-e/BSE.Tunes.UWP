using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ServiceUrlSettingsPageViewModel : ViewModelBase
    {
        private SettingsService _settingsService => SettingsService.Instance;
        private ICommand _removeUrlCommand;
        private string _strServiceUrl;

        public string ServiceUrl
        {
            get
            {
                return _strServiceUrl;
            }
            set
            {
                _strServiceUrl = value;
            }
        }

        public ICommand RemoveUrlCommand => _removeUrlCommand ?? (_removeUrlCommand = new RelayCommand(RemoveUrl));

        public ServiceUrlSettingsPageViewModel()
        {
            ServiceUrl = _settingsService.ServiceUrl;
        }
        
        public async void RemoveUrl()
        {
            _settingsService.ServiceUrl = null;
            await NavigationService.NavigateAsync(typeof(Views.ServiceUrlWizzardPage), navitageFullscreen:true);
        }
    }
}
