using BSE.Tunes.StoreApp.Models.Contract;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistEntriesChangedArgs : PlaylistChangedArgs
    {
        public PlaylistEntriesChangedArgs(Playlist playlist) : base(playlist)
        {
        }
    }
}
