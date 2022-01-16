using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistCreatedArgs : PlaylistChangedArgs
    {
        public InsertMode InsertMode
        {
            get; private set;
        }
        public PlaylistCreatedArgs(Playlist playlist, InsertMode insertMode) : base(playlist)
        {
            InsertMode = insertMode;
        }
    }
}
