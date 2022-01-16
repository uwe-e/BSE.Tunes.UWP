using System;

namespace BSE.Tunes.StoreApp.Models.Contract
{
    public class PlaylistEntry
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public int PlaylistId { get; set; }

        public int SortOrder { get; set; }

        public Guid AlbumId { get; set; }

        public int TrackId { get; set; }

        public Track Track { get; set; }

    }
}
