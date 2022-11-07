using BSE.Tunes.StoreApp.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class MenuFlyoutItemViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public event EventHandler<EventArgs> ItemClicked;

        private string _text;
        private string _icon;
        private bool _isSeparator;
        private ICommand _menuItemClickedCommand;
        private InsertMode _insertMode;
        private string _glyph;

        public bool IsSeparator
        {
            get
            {
                return _isSeparator;
            }
            set
            {
                _isSeparator = value;
                RaisePropertyChanged(nameof(IsSeparator));
            }
        }
        public InsertMode InsertMode
        {
            get
            {
                return _insertMode;
            }
            set
            {
                _insertMode = value;
                RaisePropertyChanged(nameof(InsertMode));
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                RaisePropertyChanged(nameof(Icon));
            }
        }

        public string Glyph
        {
            get
            {
                return _glyph;
            }
            set
            {
                _glyph = value;
                RaisePropertyChanged(nameof(Glyph));
            }
        }

        public dynamic Data
        {
            get;
            set;
        }

        public ICommand MenuItemClickedCommand => _menuItemClickedCommand ?? (_menuItemClickedCommand = new RelayCommand<MenuFlyoutItemViewModel>(MenuItemClicked));

        private void MenuItemClicked(MenuFlyoutItemViewModel obj)
        {
            ItemClicked?.Invoke(obj, EventArgs.Empty);
        }
    }
}
