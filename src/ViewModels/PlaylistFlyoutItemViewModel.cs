using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistFlyoutItemViewModel : MenuFlyoutItemViewModel
    {
        private Playlist m_playlist;
        private ICommand m_addSelectedToPlaylistCommand;

        public Playlist Playlist
        {
            get
            {
                return m_playlist;
            }
            set
            {
                m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
        public ICommand AddSelectedToPlaylistCommand => m_addSelectedToPlaylistCommand ?? (m_addSelectedToPlaylistCommand = new RelayCommand<object>(AddSelectedToPlaylist));

        private void AddSelectedToPlaylist(object obj)
        {
            //throw new NotImplementedException();
        }
    }
}
