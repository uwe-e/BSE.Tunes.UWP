using System;

namespace BSE.Tunes.StoreApp.Models.Contract
{
    public class Track
    {
        public int Id
        {
            get; set;
        }

        public int TrackNumber
        {
            get; set;
        }

        public Album Album
        {
            get; set;
        }

        public TimeSpan Duration
        {
            get; set;
        }

        public Guid Guid
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }
    }
}
