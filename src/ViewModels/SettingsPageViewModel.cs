using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        SettingsService m_settings;
        private bool m_themeSelectionHasChanged;

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand => _switchThemeCommand ?? (_switchThemeCommand = new RelayCommand<ElementTheme>(async (param) =>
        {
            ElementTheme = param;
            await ThemeSelectorService.SetThemeAsync(param);
        }));

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        public SettingsPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_settings = SettingsService.Instance;
            }
        }

        public bool ThemeSelectionHasChanged
        {
            get
            {
                return m_themeSelectionHasChanged;
            }
            set
            {
                m_themeSelectionHasChanged = value;
                base.RaisePropertyChanged();
            }
        }
    }
}

