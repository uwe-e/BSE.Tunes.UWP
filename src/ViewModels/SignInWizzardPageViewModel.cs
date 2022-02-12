using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SignInWizzardPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IAuthenticationService _authenticationService => AuthenticationService.Instance;
        private IDialogService _dialogSService => DialogService.Instance;
        private SettingsService _settingsService => SettingsService.Instance;
        private RelayCommand _authenticateCommand;
        private string _userName;
        private string _password;
        private bool _useSecureLogin;
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                this._userName = value;
                this.AuthenticateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("UserName");
            }
        }
        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
                this.AuthenticateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("Password");
            }
        }
        public RelayCommand AuthenticateCommand
        {
            get
            {
                return this._authenticateCommand ??
                    (this._authenticateCommand = new RelayCommand(this.Authenticate, this.CanExecuteAuthenticateCommand));
            }
        }
        #endregion

        #region MethodsPublic
        public SignInWizzardPageViewModel()
        {
            UserName = _settingsService.User?.UserName; 
        }
        #endregion

        #region MethodsPrivate
        private bool CanExecuteAuthenticateCommand()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }
        private async void Authenticate()
        {
            try
            {
                var user = await this._authenticationService.AuthenticateAsync(UserName, Password).ConfigureAwait(true);
                //Clears the cache with the back stack before navigate
                //NavigationService.ClearCache(true);
                await NavigationService.NavigateAsync(typeof(Views.MainPage));
            }
            catch (Exception exception)
            {
                await _dialogSService.ShowMessageDialogAsync(exception.Message, ResourceService.GetString("ExceptionMessageDialogHeader", "Error"));
            }
        }
        #endregion
    }
}
