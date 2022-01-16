using BSE.Tunes.StoreApp.Models.Contract;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistDeletedArgs : PlaylistChangedArgs
    {
        public PlaylistDeletedArgs(Playlist playlist) : base(playlist)
        {
        }
    }
}
