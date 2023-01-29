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

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            Track track = (Track)obj;
            if (!track.Id.Equals(Id))
            {
                return false;
            }
            if (!track.Guid.Equals(Guid))
            {
                return false;
            }
            return true;

        }
    }
}
