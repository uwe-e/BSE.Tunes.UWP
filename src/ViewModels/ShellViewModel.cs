using BSE.Tunes.StoreApp.Helpers;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool _isBackEnabled;
        private WinUI.NavigationView _navigationView;
        private WinUI.NavigationViewItem _selected;

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

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
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

        public void Initialize(Frame frame, WinUI.NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _navigationView = navigationView;
            //_keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            NavigationService.Navigated += OnFrameNavigated;
            //NavigationService.NavigationFailed += Frame_NavigationFailed;
            //NavigationService.Navigated += Frame_Navigated;
            //_navigationView.BackRequested += OnBackRequested;
        }

        private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            var selectedItem = GetSelectedItem(_navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IList<object> menuItems, Type sourcePageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, sourcePageType))
                {
                    return item;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem item, Type sourcePageType)
        {
            var navigatedPageKey = NavigationService.GetNameOfRegisteredPage(sourcePageType);
            var pageType = item.GetValue(NavHelper.PageTypeProperty) as Type;
            //return pageKey == navigatedPageKey;
            return pageType.Equals(sourcePageType);
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
