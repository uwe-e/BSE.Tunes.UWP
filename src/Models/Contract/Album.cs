using System;

namespace BSE.Tunes.StoreApp.Models.Contract
{
    public class Album
    {
        public int Id
        {
            get; set;
        }

        public Guid AlbumId
        {
            get; set;
        }

        public Artist Artist
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public int? Year
        {
            get; set;
        }

        public Genre Genre
        {
            get;
            set;
        }

        public Track[] Tracks
        {
            get; set;
        }

    }
}
