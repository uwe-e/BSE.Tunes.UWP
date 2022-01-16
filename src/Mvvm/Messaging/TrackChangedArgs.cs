using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Messaging;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class TrackChangedArgs : MessageBase
    {
        public Track Track { get; private set; }
        public TrackChangedArgs(Track track)
        {
            this.Track = track;
        }
    }
}
