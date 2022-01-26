using BSE.Tunes.StoreApp.Mvvm;
using System.Collections.ObjectModel;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SettingsMainPageViewModel: ViewModelBase
    {
        #region FieldsPrivate
        private ISettingsItemViewModel m_settingsHostItemViewModel;
        #endregion

        #region Properties
        public ObservableCollection<ISettingsItemViewModel> SettingItems { get; } = new ObservableCollection<ISettingsItemViewModel>();

        public ISettingsItemViewModel SelectedItem
        {
            get
            {
                return m_settingsHostItemViewModel;
            }
            set
            {
                m_settingsHostItemViewModel = value;
                RaisePropertyChanged("SelectedItem");
            }
        }
        #endregion

        #region MethodsPublic
        public SettingsMainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SettingItems.Add(new ServiceUrlSettingsItemViewModel());
                SettingItems.Add(new SignOutSettingsItemViewModel());
                SettingItems.Add(new SystemSettingsItemViewModel());
                SettingItems.Add(new AboutItemViewModel());
            }
        }
        #endregion
    }
}
