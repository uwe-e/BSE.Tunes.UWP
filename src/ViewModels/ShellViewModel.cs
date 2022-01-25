using BSE.Tunes.StoreApp.Helpers;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ShellViewModel : SearchSuggestionsViewModel
    {
        private SettingsService m_settingsService;
        private bool m_isHamburgerMenuOpen;
        private ICommand m_isOpenChangedCommand;
        private ICommand m_openMenuPanelCommand;
        private ICommand _itemInvokedCommand;
        private ICommand _backRequestedCommand;

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

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ICommand BackRequestedCommand => _backRequestedCommand ?? (_backRequestedCommand = new RelayCommand<WinUI.NavigationViewBackRequestedEventArgs>(OnBackRequested));

        


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

        private async void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                await NavigationService.NavigateAsync(typeof(Views.SettingsMainPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem)
                {
                    var pagetype = selectedItem?.GetValue(NavHelper.PageTypeProperty) as Type;

                    if (pagetype != null)
                    {
                        await NavigationService.NavigateAsync(pagetype, null, args.RecommendedNavigationTransitionInfo);
                    }
                }
            }
        }
        
        private void OnBackRequested(WinUI.NavigationViewBackRequestedEventArgs obj)
        {
            NavigationService.GoBack();
        }
    }
}
