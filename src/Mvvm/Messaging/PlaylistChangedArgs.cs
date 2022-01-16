using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Messaging;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistChangedArgs : MessageBase
    {
        public Playlist Playlist
        {
            get; private set;
        }
        public PlaylistChangedArgs(Playlist playlist)
        {
            Playlist = playlist;
        }
    }
}
