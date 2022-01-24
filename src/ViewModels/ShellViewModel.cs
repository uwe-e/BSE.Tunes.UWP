using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ShellViewModel : SearchSuggestionsViewModel
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        private bool m_isHamburgerMenuOpen;
        private ICommand m_isOpenChangedCommand;
        private ICommand m_openMenuPanelCommand;
        #endregion

        #region Properties
        public bool IsHamburgerMenuOpen
        {
            get
            {
                return m_isHamburgerMenuOpen;
            }
            set
            {
                m_isHamburgerMenuOpen = value;
                RaisePropertyChanged(() => IsHamburgerMenuOpen);
            }
        }
        public ICommand OpenMenuPanelCommand => m_openMenuPanelCommand ?? (m_openMenuPanelCommand = new RelayCommand(() =>
        {
            IsHamburgerMenuOpen = true;
        }));

        public ICommand IsOpenChangedCommand => m_isOpenChangedCommand ?? (m_isOpenChangedCommand = new RelayCommand<bool>((isOpen) =>
       {
           m_settingsService.IsHamburgerMenuOpen = isOpen;
       }));
        #endregion

        public ShellViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_settingsService = SettingsService.Instance;
            }
        }

        public void Initialize(Frame frame, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            //_navigationView = navigationView;
            //_keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            //NavigationService.NavigationFailed += Frame_NavigationFailed;
            //NavigationService.Navigated += Frame_Navigated;
            //_navigationView.BackRequested += OnBackRequested;
        }


    }
}
