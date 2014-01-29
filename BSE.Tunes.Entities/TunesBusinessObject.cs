﻿using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BSE.Tunes.Data.Exceptions;
using MySql.Data.MySqlClient;
using System.Globalization;
using BSE.Tunes.Entities.Properties;

namespace BSE.Tunes.Entities
{
    public class TunesBusinessObject : ITunesService
    {
        #region FieldsPrivate
        private string m_strAudioDirectory;
        private string m_strConnection;
        #endregion

        #region Properties
        public string AudioDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_strAudioDirectory) == true)
                {
                    this.m_strAudioDirectory = System.Configuration.ConfigurationManager.AppSettings["AudioDirectory"];
                }
                return this.m_strAudioDirectory;
            }
        }
        protected string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_strConnection) == true)
                {
                    this.m_strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["TunesEntities"].ConnectionString;
                }
                return this.m_strConnection;
            }
        }
        #endregion

        #region MethodsPublic
        public TunesBusinessObject()
        {
        }

        public bool IsHostAccessible()
        {
            bool isAccessible = false;
            using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
            {
                using (System.Data.Objects.ObjectContext objectContext = tunesEntity.ObjectContext())
                {
                    isAccessible = objectContext.DatabaseExists();
                }
            }
            return isAccessible;
        }

        public Genre[] GetGenres()
        {
            Genre[] genres = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT g.genreid, g.genre1 FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.genre AS g ON a.genreid = g.genreid");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
            stringBuilder.Append(" GROUP BY g.genreid, g.genre1");
            stringBuilder.Append(" ORDER BY g.genre1");

            using (System.Data.EntityClient.EntityConnection entityConnection =
                    new System.Data.EntityClient.EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        List<Genre> genreCollection = null;
                        entityCommand.CommandText = stringBuilder.ToString();
                        // Execute the command.
                        using (EntityDataReader dataReader =
                            entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            // Start reading results.
                            while (dataReader.Read())
                            {
                                IDataReader dataR = dataReader;
                                if (genreCollection == null)
                                {
                                    genreCollection = new List<Genre>();
                                }
                                Genre genre = new Genre
                                {
                                    Id = dataReader.GetInt32("genreid", false, 0),
                                    Name = dataReader.GetString("genre1", false, string.Empty)
                                };
                                genreCollection.Add(genre);
                            }
                        }
                        if (genreCollection != null)
                        {
                            genres = genreCollection.ToArray();
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return genres;
        }

        public int GetNumberOfPlayableAlbums()
        {
            int iNumberofAlbums = -1;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(DISTINCT a.TitelID) FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");

            string sql = stringBuilder.ToString();
            using (System.Data.EntityClient.EntityConnection entityConnection =
                    new System.Data.EntityClient.EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            iNumberofAlbums = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return iNumberofAlbums;
        }

        public Album[] GetAlbums(Query query)
        {
            Album[] albums = null;
            if (query == null)
            {
                query = new Query
                {
                    PageIndex = 0,
                    PageSize = 1
                };
            }

            query.PageSize = query.PageSize == 0 ? 1 : query.PageSize;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.thumbnail FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
            stringBuilder.Append(" GROUP BY i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.thumbnail");
            if (query.SortByCondition != null && query.SortByCondition.Id == 1)
            {
                stringBuilder.Append(" ORDER BY a.Titel1");
            }
            else
            {
                stringBuilder.Append(" ORDER BY i.Interpret, a.Titel1");
            }
            stringBuilder.Append(" SKIP @skip LIMIT @limit ");

            string sql = stringBuilder.ToString();

            using (System.Data.EntityClient.EntityConnection entityConnection =
                new System.Data.EntityClient.EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        EntityParameter skip = new EntityParameter();
                        skip.ParameterName = "skip";
                        skip.Value = query.PageIndex;
                        entityCommand.Parameters.Add(skip);

                        EntityParameter limit = new EntityParameter();
                        limit.ParameterName = "limit";
                        limit.Value = query.PageSize;
                        entityCommand.Parameters.Add(limit);

                        List<Album> albumCollection = null;
                        entityCommand.CommandText = sql;
                        // Execute the command.
                        using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            // Start reading results.
                            while (dataReader.Read())
                            {
                                if (albumCollection == null)
                                {
                                    albumCollection = new List<Album>();
                                }
                                Album album = new Album
                                {
                                    Artist = new Artist
                                    {
                                        Id = dataReader.GetInt32("InterpretID", false, 0),
                                        Name = dataReader.GetString("Interpret", false, string.Empty),
                                        SortName = dataReader.GetString("Interpret_Lang", true, string.Empty)
                                    },
                                    Id = dataReader.GetInt32("TitelID", false, 0),
                                    Title = dataReader.GetString("Titel1", false, string.Empty),
                                    Cover = dataReader.GetBytes("thumbnail", true, null)

                                };
                                albumCollection.Add(album);
                            }
                        }
                        if (albumCollection != null)
                        {
                            albums = albumCollection.ToArray();
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return albums;
        }

        public Album[] GetNewestAlbums(int limit)
        {
            Album[] albums = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.thumbnail FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
            stringBuilder.Append(" ORDER BY a.TitelID DESC");
            stringBuilder.Append(" LIMIT @limit ");

            string sql = stringBuilder.ToString();

            using (System.Data.EntityClient.EntityConnection entityConnection =
                new System.Data.EntityClient.EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        EntityParameter limitParam = new EntityParameter();
                        limitParam.ParameterName = "limit";
                        limitParam.Value = limit;
                        entityCommand.Parameters.Add(limitParam);

                        List<Album> albumCollection = null;
                        entityCommand.CommandText = sql;
                        // Execute the command.
                        using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            // Start reading results.
                            while (dataReader.Read())
                            {
                                if (albumCollection == null)
                                {
                                    albumCollection = new List<Album>();
                                }
                                Album album = new Album
                                {
                                    Artist = new Artist
                                    {
                                        Id = dataReader.GetInt32("InterpretID", false, 0),
                                        Name = dataReader.GetString("Interpret", false, string.Empty),
                                        SortName = dataReader.GetString("Interpret_Lang", true, string.Empty)
                                    },
                                    Id = dataReader.GetInt32("TitelID", false, 0),
                                    Title = dataReader.GetString("Titel1", false, string.Empty),
                                    Cover = dataReader.GetBytes("thumbnail", true, null)

                                };
                                albumCollection.Add(album);
                            }
                        }
                        if (albumCollection != null)
                        {
                            albums = albumCollection.ToArray();
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return albums;
        }

        public Album GetAlbumById(int albumId)
        {
            Album album = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.cover, a.ErschDatum, g.genreid, g.genre1");
            stringBuilder.Append(" FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
            stringBuilder.Append(" LEFT JOIN tunesEntities.genre AS g ON a.genreid = g.genreid");
            stringBuilder.Append(" WHERE a.titelid = @albumId");

            string sql = stringBuilder.ToString();

            using (System.Data.EntityClient.EntityConnection entityConnection =
                    new System.Data.EntityClient.EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        EntityParameter id = new EntityParameter();
                        id.ParameterName = "albumid";
                        id.Value = albumId;
                        entityCommand.Parameters.Add(id);
                        entityCommand.CommandText = sql;
                        using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            if (dataReader.Read())
                            {
                                album = new Album
                                {
                                    Artist = new Artist
                                    {
                                        Id = dataReader.GetInt32("InterpretID", false, 0),
                                        Name = dataReader.GetString("Interpret", false, string.Empty),
                                        SortName = dataReader.GetString("Interpret_Lang", true, string.Empty)
                                    },
                                    Id = dataReader.GetInt32("TitelID", false, 0),
                                    Title = dataReader.GetString("Titel1", false, string.Empty),
                                    Cover = dataReader.GetBytes("cover", true, null),
                                    Year = dataReader.GetInt32("ErschDatum", true, 0),
                                    Genre = new Genre
                                    {
                                        Id = dataReader.GetInt32("genreid", true, 0),
                                        Name = dataReader.GetString("genre1", true, string.Empty)
                                    }
                                };
                            }
                        }
                        GetAlbumTracksByTitelId(album, entityConnection);
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return album;
        }
        public string GetAudioFileNameByGuid(Guid guid)
        {
            string fileName = null;
            string audioDirectory = this.AudioDirectory;
            if (string.IsNullOrEmpty(audioDirectory) == false && guid != null && guid != Guid.Empty)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.Liedpfad FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" WHERE t.guid = @guid");

                string sql = stringBuilder.ToString();
                using (System.Data.EntityClient.EntityConnection entityConnection =
                        new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter guidParam = new EntityParameter();
                            guidParam.ParameterName = "guid";
                            guidParam.Value = guid.ToString();
                            entityCommand.Parameters.Add(guidParam);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    fileName = GetTrackFilePath(dataReader, audioDirectory);
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return fileName;
        }
        public Track GetTrackById(int trackId)
        {
            Track track = null;
            string audioDirectory = this.AudioDirectory;
            if (string.IsNullOrEmpty(audioDirectory) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer, t.Liedpfad, t.guid, a.TitelID, a.Titel1, a.Cover, i.Interpret FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" JOIN tunesEntities.titel AS a ON a.TitelID = t.TitelID");
                stringBuilder.Append(" JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
                stringBuilder.Append(" WHERE t.LiedId = @trackid");

                string sql = stringBuilder.ToString();
                using (System.Data.EntityClient.EntityConnection entityConnection =
                        new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter trackIdParam = new EntityParameter();
                            trackIdParam.ParameterName = "trackid";
                            trackIdParam.Value = trackId;
                            entityCommand.Parameters.Add(trackIdParam);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    track = new Track
                                    {
                                        Id = dataReader.GetInt32("LiedID", false, 0),
                                        TrackNumber = dataReader.GetInt32("Track", false, 0),
                                        Name = dataReader.GetString("Lied", false, string.Empty),
                                        Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue),
                                        Path = GetTrackFilePath(dataReader, audioDirectory),
                                        Guid = dataReader.GetGuid("guid", false, Guid.Empty),
                                        Album = new Album
                                        {
                                            Id = dataReader.GetInt32("TitelID", false, 0),
                                            Title = dataReader.GetString("Titel1", false, string.Empty),
                                            Cover = dataReader.GetBytes("Cover", true, null),
                                            Artist = new Artist
                                            {
                                                Name = dataReader.GetString("Interpret", false, string.Empty)
                                            }
                                        }
                                    };
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return track;
        }
        public ICollection<Track> GetTracksByFilters(Filter filter)
        {
            Collection<Track> tracks = null;
            if (filter != null)
            {
                //filter.Value = "17,25,5,14";
                var names = new int[] { 17, 25, 5, 14 };

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer");
                stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" INNER JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
                //stringBuilder.Append(" AND a.genreid IN (17,25,5,14)");
                
                switch (filter.Mode)
                {
                    case FilterMode.Genre:
                        stringBuilder.Append(" AND a.genreid IN {" + filter.Value + "}");
                        //stringBuilder.Append(" AND a.genreid IN (@filterValue)");
                        break;
                    case FilterMode.Year:
                        break;
                    default:
                        break;
                }

                string sql = stringBuilder.ToString();
                using (System.Data.EntityClient.EntityConnection entityConnection =
                   new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            //EntityParameter filterValue = new EntityParameter();
                            //filterValue.ParameterName = "filterValue";
                            //filterValue.Value = filter.Value;
                            //entityCommand.Parameters.Add(filterValue);
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (tracks == null)
                                    {
                                        tracks = new Collection<Track>();
                                    }
                                    Track track = new Track
                                    {
                                        Id = dataReader.GetInt32("LiedID", false, 0)
                                    };
                                    tracks.Add(track);
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }

            }
            return tracks;
        }
        public BSE.Tunes.Data.SearchResult GetSearchResults(Query query)
        {
            BSE.Tunes.Data.SearchResult searchResult = new Data.SearchResult();
            if (query!= null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                searchResult.Albums = this.GetAlbumSearchResults(query);
                searchResult.Tracks = this.GetTrackSearchResults(query);
            }
            return searchResult;
        }
        public BSE.Tunes.Data.SearchResult GetSearchResults(string queryString, int pageIndex, int pageSize)
        {
            BSE.Tunes.Data.SearchResult searchResult = new Data.SearchResult();
            if (string.IsNullOrEmpty(queryString) == false)
            {
                searchResult.Albums = this.GetAlbumSearchResults(queryString, pageIndex, pageSize);
                searchResult.Tracks = this.GetTrackSearchResults(queryString, pageIndex, pageSize);
            }
            return searchResult;
        }
        public Album[] GetAlbumSearchResults(Query query)
        {
            Album[] albums = null;
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                query.PageSize = query.PageSize == 0 ? 1 : query.PageSize;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT DISTINCT al.titelid AS AlbumId, a.interpretid AS ArtistId, a.interpret AS ArtistName, al.titel AS AlbumName, al.thumbnail AS Thumbnail, 0 AS TrackId, '' AS Track");
                stringBuilder.Append(" FROM titel al");
                stringBuilder.Append(" JOIN interpreten a ON al.interpretid = a.interpretid");
                stringBuilder.Append(" JOIN lieder t ON al.titelid = t.titelid AND t.liedpfad IS NOT NULL");
                stringBuilder.Append(" WHERE MATCH (a.interpret,al.titel) AGAINST (?querystring IN BOOLEAN MODE)");
                stringBuilder.Append(" ORDER BY a.interpret ,al.titel");
                stringBuilder.Append(" LIMIT ?limit OFFSET ?offset");

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        MySqlParameter paramQueryString = new MySqlParameter("querystring", MySqlDbType.VarChar, 60);
                        paramQueryString.Direction = ParameterDirection.Input;
                        paramQueryString.Value = query.SearchPhrase;

                        MySqlParameter paramLimit = new MySqlParameter("limit", MySqlDbType.Int32, 0);
                        paramLimit.Direction = ParameterDirection.Input;
                        paramLimit.Value = query.PageSize;

                        MySqlParameter paramOffset = new MySqlParameter("offset", MySqlDbType.Int32, 0);
                        paramOffset.Direction = ParameterDirection.Input;
                        paramOffset.Value = query.PageIndex;

                        List<Album> albumCollection = null;
                        using (System.Data.Objects.ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var result = objectContext.ExecuteStoreQuery<SearchResult>(stringBuilder.ToString(), paramQueryString, paramLimit, paramOffset);
                            if (result != null)
                            {
                                if (albumCollection == null)
                                {
                                    albumCollection = new List<Album>();
                                }
                                foreach (var album in result)
                                {
                                    if (album != null)
                                    {
                                        albumCollection.Add(new Album
                                        {
                                            Id = album.AlbumId,
                                            Title = album.AlbumName,
                                            Thumbnail = album.Thumbnail,
                                            Artist = new Artist
                                            {
                                                Id = album.ArtistId,
                                                Name = album.ArtistName
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (albumCollection != null)
                        {
                            albums = albumCollection.ToArray();
                        }
                    }
                }
            }
            return albums;
        }
        public Album[] GetAlbumSearchResults(string queryString, int pageIndex, int pageSize)
        {
            Album[] albums = null;
            if (string.IsNullOrEmpty(queryString) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT DISTINCT al.titelid AS AlbumId, a.interpretid AS ArtistId, a.interpret AS ArtistName, al.titel AS AlbumName, al.thumbnail AS Thumbnail, 0 AS TrackId, '' AS Track");
                stringBuilder.Append(" FROM titel al");
                stringBuilder.Append(" JOIN interpreten a ON al.interpretid = a.interpretid");
                stringBuilder.Append(" JOIN lieder t ON al.titelid = t.titelid AND t.liedpfad IS NOT NULL");
                stringBuilder.Append(" WHERE MATCH (a.interpret,al.titel) AGAINST (?querystring IN BOOLEAN MODE)");
                stringBuilder.Append(" ORDER BY a.interpret ,al.titel");
                stringBuilder.Append(" LIMIT ?limit OFFSET ?offset");

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        MySqlParameter paramQueryString = new MySqlParameter("querystring", MySqlDbType.VarChar, 60);
                        paramQueryString.Direction = ParameterDirection.Input;
                        paramQueryString.Value = queryString;

                        MySqlParameter paramLimit = new MySqlParameter("limit", MySqlDbType.Int32, 0);
                        paramLimit.Direction = ParameterDirection.Input;
                        paramLimit.Value = pageSize;

                        MySqlParameter paramOffset = new MySqlParameter("offset", MySqlDbType.Int32, 0);
                        paramOffset.Direction = ParameterDirection.Input;
                        paramOffset.Value = pageIndex;

                        List<Album> albumCollection = null;
                        using (System.Data.Objects.ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var result = objectContext.ExecuteStoreQuery<SearchResult>(stringBuilder.ToString(), paramQueryString, paramLimit, paramOffset);
                            if (result != null)
                            {
                                if (albumCollection == null)
                                {
                                    albumCollection = new List<Album>();
                                }
                                foreach (var album in result)
                                {
                                    if (album != null)
                                    {
                                        albumCollection.Add(new Album
                                        {
                                            Id = album.AlbumId,
                                            Title = album.AlbumName,
                                            Thumbnail = album.Thumbnail,
                                            Artist = new Artist
                                            {
                                                Id = album.ArtistId,
                                                Name = album.ArtistName
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (albumCollection != null)
                        {
                            albums = albumCollection.ToArray();
                        }
                    }
                }
            }
            return albums;
        }
        public Track[] GetTrackSearchResults(Query query)
        {
            Track[] tracks = null;
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                query.PageSize = query.PageSize == 0 ? 1 : query.PageSize;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT al.titelid AS AlbumId, a.interpretid AS ArtistId, a.interpret AS ArtistName, al.titel AS AlbumName, al.thumbnail AS Thumbnail, t.liedid as TrackId, t.lied AS Track");
                stringBuilder.Append(" FROM titel al");
                stringBuilder.Append(" JOIN interpreten a ON al.interpretid = a.interpretid");
                stringBuilder.Append(" JOIN lieder t ON al.titelid = t.titelid AND t.liedpfad IS NOT NULL");
                stringBuilder.Append(" WHERE MATCH (t.lied) AGAINST (?querystring IN BOOLEAN MODE)");
                stringBuilder.Append(" ORDER BY t.lied, a.interpret ,al.titel");
                stringBuilder.Append(" LIMIT ?limit OFFSET ?offset");

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        MySqlParameter paramQueryString = new MySqlParameter("querystring", MySqlDbType.VarChar, 60);
                        paramQueryString.Direction = ParameterDirection.Input;
                        paramQueryString.Value = query.SearchPhrase;

                        MySqlParameter paramLimit = new MySqlParameter("limit", MySqlDbType.Int32, 0);
                        paramLimit.Direction = ParameterDirection.Input;
                        paramLimit.Value = query.PageSize;

                        MySqlParameter paramOffset = new MySqlParameter("offset", MySqlDbType.Int32, 0);
                        paramOffset.Direction = ParameterDirection.Input;
                        paramOffset.Value = query.PageIndex;

                        List<Track> trackCollection = null;
                        using (System.Data.Objects.ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var results = objectContext.ExecuteStoreQuery<SearchResult>(stringBuilder.ToString(), paramQueryString, paramLimit, paramOffset);
                            if (results != null)
                            {
                                if (trackCollection == null)
                                {
                                    trackCollection = new List<Track>();
                                }
                                foreach (var result in results)
                                {
                                    if (result != null)
                                    {
                                        trackCollection.Add(new Track
                                        {
                                            Id = result.TrackId,
                                            Name = result.Track,
                                            Album = new Album
                                            {
                                                Id = result.AlbumId,
                                                Title = result.AlbumName,
                                                Thumbnail = result.Thumbnail,
                                                Artist = new Artist
                                                {
                                                    Id = result.ArtistId,
                                                    Name = result.ArtistName
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (trackCollection != null)
                        {
                            tracks = trackCollection.ToArray();
                        }
                    }
                }
            }
            return tracks;
        }
        public Track[] GetTrackSearchResults(string queryString, int pageIndex, int pageSize)
        {
            Track[] tracks = null;
            if (string.IsNullOrEmpty(queryString) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT al.titelid AS AlbumId, a.interpretid AS ArtistId, a.interpret AS ArtistName, al.titel AS AlbumName, al.thumbnail AS Thumbnail, t.liedid as TrackId, t.lied AS Track");
                stringBuilder.Append(" FROM titel al");
                stringBuilder.Append(" JOIN interpreten a ON al.interpretid = a.interpretid");
                stringBuilder.Append(" JOIN lieder t ON al.titelid = t.titelid AND t.liedpfad IS NOT NULL");
                stringBuilder.Append(" WHERE MATCH (t.lied) AGAINST (?querystring IN BOOLEAN MODE)");
                stringBuilder.Append(" ORDER BY t.lied, a.interpret ,al.titel");
                stringBuilder.Append(" LIMIT ?limit OFFSET ?offset");

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        MySqlParameter paramQueryString = new MySqlParameter("querystring", MySqlDbType.VarChar, 60);
                        paramQueryString.Direction = ParameterDirection.Input;
                        paramQueryString.Value = queryString;

                        MySqlParameter paramLimit = new MySqlParameter("limit", MySqlDbType.Int32, 0);
                        paramLimit.Direction = ParameterDirection.Input;
                        paramLimit.Value = pageSize;

                        MySqlParameter paramOffset = new MySqlParameter("offset", MySqlDbType.Int32, 0);
                        paramOffset.Direction = ParameterDirection.Input;
                        paramOffset.Value = pageIndex;

                        List<Track> trackCollection = null;
                        using (System.Data.Objects.ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var results = objectContext.ExecuteStoreQuery<SearchResult>(stringBuilder.ToString(), paramQueryString, paramLimit, paramOffset);
                            if (results != null)
                            {
                                if (trackCollection == null)
                                {
                                    trackCollection = new List<Track>();
                                }
                                foreach (var result in results)
                                {
                                    if (result != null)
                                    {
                                        trackCollection.Add(new Track
                                        {
                                            Id = result.TrackId,
                                            Name = result.Track,
                                            Album = new Album
                                            {
                                                Id = result.AlbumId,
                                                Title = result.AlbumName,
                                                Thumbnail = result.Thumbnail,
                                                Artist = new Artist
                                                {
                                                    Id = result.ArtistId,
                                                    Name = result.ArtistName
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (trackCollection != null)
                        {
                            tracks = trackCollection.ToArray();
                        }
                    }
                }
            }
            return tracks;
        }

        public void UpdateHistory(History history)
        {
            if (history != null)
            {
                HistoryEntity entity = new HistoryEntity
                {
                    AppID = history.AppId,
                    TitelID = history.AlbumId,
                    LiedID = history.TrackId,
                    Zeit = history.PlayedAt,
                    Benutzer = history.UserName,
                    Interpret = string.Empty,
                    Titel = string.Empty,
                    Lied = string.Empty
                };

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    tunesEntity.history.Add(entity);
                    tunesEntity.SaveChanges();
                }
            }
        }
        public Playlist GetPlaylistById(int playlistId, string userName)
        {
            Playlist playlist = null;
            if (string.IsNullOrEmpty(userName) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.User, pe.EntryId, pe.sortorder, t.LiedID, t.Lied, t.Dauer, i.Interpret FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" LEFT JOIN tunesEntities.lieder AS t ON pe.LiedId = t.LiedID");
                stringBuilder.Append(" LEFT JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" LEFT JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
                stringBuilder.Append(" WHERE p.ListId = @playlistId");
                stringBuilder.Append(" AND p.User = @userName");

                string sql = stringBuilder.ToString();
                using (System.Data.EntityClient.EntityConnection entityConnection =
                        new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter trackIdParam = new EntityParameter();
                            trackIdParam.ParameterName = "playlistId";
                            trackIdParam.Value = playlistId;
                            entityCommand.Parameters.Add(trackIdParam);

                            EntityParameter user = new EntityParameter();
                            user.ParameterName = "userName";
                            user.Value = userName;
                            entityCommand.Parameters.Add(user);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (playlist == null)
                                    {
                                        playlist = new Playlist
                                        {
                                            Id = dataReader.GetInt32("ListId", false, 0),
                                            Name = dataReader.GetString("ListName", false, string.Empty),
                                            UserName = dataReader.GetString("User", false, string.Empty)
                                        };
                                    }
                                    int entryId = dataReader.GetInt32("EntryId", true, 0);
                                    if (entryId > 0)
                                    {
                                        PlaylistEntry entry = new PlaylistEntry
                                        {
                                            Id = entryId,
                                            SortOrder = dataReader.GetInt32("sortorder", true, 0),
                                            TrackId = dataReader.GetInt32("LiedID", true, 0),
                                            Name = dataReader.GetString("Lied", true, string.Empty),
                                            Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue),
                                            Artist = dataReader.GetString("Interpret", true, string.Empty)
                                        };
                                        playlist.Entries.Add(entry);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlist;
        }
        public Playlist[] GetPlaylistsByUserName(string userName)
        {
            return GetPlaylistsByUserName(userName, 0);
        }
        public Playlist[] GetPlaylistsByUserName(string userName, int limit)
        {
            Playlist[] playlists = null;
            if (!string.IsNullOrEmpty(userName))
            {
                bool hasLimit = false;
                if (limit > 0)
                {
                    hasLimit = true;
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.User ,p.guid FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" WHERE p.User = @userName");
                stringBuilder.Append(" ORDER BY p.ListName");
                if (hasLimit)
                {
                    stringBuilder.Append(" LIMIT @limit ");
                }
                string sql = stringBuilder.ToString();

                using (System.Data.EntityClient.EntityConnection entityConnection =
                    new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter user = new EntityParameter();
                            user.ParameterName = "userName";
                            user.Value = userName;
                            entityCommand.Parameters.Add(user);

                            EntityParameter limitParam = new EntityParameter();
                            limitParam.ParameterName = "limit";
                            limitParam.Value = limit;
                            entityCommand.Parameters.Add(limitParam);

                            List<Playlist> playlistCollection = null;
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (playlistCollection == null)
                                    {
                                        playlistCollection = new List<Playlist>();
                                    }

                                    Playlist playlist = new Playlist
                                    {
                                        Id = dataReader.GetInt32("ListId", false, 0),
                                        Name = dataReader.GetString("ListName", false, string.Empty),
                                        UserName = dataReader.GetString("User", false, string.Empty)
                                    };
                                    playlistCollection.Add(playlist);
                                }
                            }
                            if (playlistCollection != null)
                            {
                                playlists = playlistCollection.ToArray();

                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlists;
        }
        public Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            Playlist playlist = null;
            if (string.IsNullOrEmpty(userName) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, COUNT(pe.PlaylistId) as Number  FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" WHERE p.ListId = @playlistId");
                stringBuilder.Append(" AND p.User = @userName");
                stringBuilder.Append(" GROUP BY p.listid, p.ListName");

                string sql = stringBuilder.ToString();
                using (System.Data.EntityClient.EntityConnection entityConnection =
                        new System.Data.EntityClient.EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter id = new EntityParameter();
                            id.ParameterName = "playlistId";
                            id.Value = playlistId;
                            entityCommand.Parameters.Add(id);

                            EntityParameter user = new EntityParameter();
                            user.ParameterName = "userName";
                            user.Value = userName;
                            entityCommand.Parameters.Add(user);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    playlist = new Playlist
                                    {
                                        Id = dataReader.GetInt32("ListId", false, 0),
                                        Name = dataReader.GetString("ListName", false, string.Empty),
                                        NumberEntries = dataReader.GetInt32("Number", false, 0)
                                    };
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlist;
        }
        public Playlist InsertPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                PlaylistEntity entity = new PlaylistEntity
                {
                    ListId = playlist.Id,
                    ListName = playlist.Name,
                    User = playlist.UserName,
                    guid = playlist.Guid.ToString(),
                    Timestamp = DateTime.Now
                };
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    PlaylistEntity playlistEntity = tunesEntity.playlist.FirstOrDefault(pl => pl.ListId == playlist.Id);
                    if (playlistEntity == null)
                    {
                        if (tunesEntity.playlist.FirstOrDefault(
                            pl => string.Compare(pl.ListName, entity.ListName) == 0 && string.Compare(pl.User, entity.User) == 0) != null)
                        {
                            string playlistExistsExceptionMessage = string.Format(CultureInfo.InvariantCulture, SRResources.PlaylistExistsException, playlist.Name);
                            throw new PlaylistExistsException(playlistExistsExceptionMessage);
                        }
                        tunesEntity.playlist.Add(entity);
                    }
                    tunesEntity.SaveChanges();
                    playlist = new Playlist
                    {
                        Id = entity.ListId,
                        Name = entity.ListName,
                        UserName = entity.User,
                        Guid = new Guid(entity.guid)
                    };
                }
            }
            return playlist;
        }

        public Playlist AppendToPlaylist(Playlist playlist)
        {
            if (playlist != null && playlist.Entries != null)
            {
                List<PlaylistEntry> entries = new List<PlaylistEntry>(playlist.Entries);
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        int sortOrder = tunesEntity.playlistentries.Where(pe => pe.PlaylistId == playlist.Id).Count();
                        entries.ForEach((playlistEntry) =>
                            {
                                if (playlistEntry != null)
                                {
                                    tunesEntity.playlistentries.Add(new PlaylistEntryEntity
                                    {
                                        Guid = playlistEntry.Guid,
                                        LiedId = playlistEntry.TrackId,
                                        PlaylistId = playlist.Id,
                                        sortorder = sortOrder,
                                        Timestamp = DateTime.Now
                                    });
                                }
                            });
                        tunesEntity.SaveChanges();
                    }
                }
            }
            return playlist;
        }

        public bool UpdatePlaylistEntries(Playlist playlist)
        {
            bool hasUpdated = false;
            if (playlist != null && playlist.Entries != null)
            {
                List<PlaylistEntry> entries = new List<PlaylistEntry>(playlist.Entries);
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        var entr = tunesEntity.playlistentries.Where((e) => e.PlaylistId == playlist.Id).ToList();
                        entr.ForEach((e) =>
                        {
                            if (e != null)
                            {
                                tunesEntity.playlistentries.Remove(e);
                            }
                        });
                        foreach (var e in playlist.Entries)
                        {
                            if (e != null)
                            {
                                PlaylistEntryEntity entity = new PlaylistEntryEntity
                                {
                                    Guid = e.Guid,
                                    LiedId = e.TrackId,
                                    PlaylistId = playlist.Id,
                                    Timestamp = DateTime.Now,
                                    sortorder = e.SortOrder
                                };
                                tunesEntity.playlistentries.Add(entity);
                            }
                        }
                        tunesEntity.SaveChanges();
                        hasUpdated = true;
                    }
                }
            }
            return hasUpdated;
        }

        public bool DeletePlaylists(IList<Playlist> playlists)
        {
            bool hasDeleted = false;
            if (playlists != null)
            {
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    foreach (var playlist in playlists)
                    {
                        if (playlist != null)
                        {
                            var entryEntities = tunesEntity.playlistentries.Where(entry => entry.PlaylistId == playlist.Id);
                            if (entryEntities != null)
                            {
                                foreach (var entry in entryEntities)
                                {
                                    if (entry != null)
                                    {
                                        tunesEntity.playlistentries.Remove(entry);
                                    }
                                }
                            }
                            var playlistEntity = tunesEntity.playlist.Where(list => list.ListId == playlist.Id).FirstOrDefault();
                            if (playlistEntity != null)
                            {
                                tunesEntity.playlist.Remove(playlistEntity);
                            }
                        }
                    }
                    tunesEntity.SaveChanges();
                    hasDeleted = true;
                }
            }
            return hasDeleted;
        }
        
        public string GetHelloWorld()
        {
            return "Hello World";
        }
        #endregion

        #region MethodsPrivate
        private void GetAlbumTracksByTitelId(Album album, System.Data.EntityClient.EntityConnection entityConnection)
        {
            if (album != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer");
                stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" WHERE t.titelid = @albumId");
                stringBuilder.Append(" AND t.Liedpfad IS NOT NULL");
                stringBuilder.Append(" ORDER BY t.Track");
                string sql = stringBuilder.ToString();

                using (EntityCommand entityCommand = entityConnection.CreateCommand())
                {
                    EntityParameter id = new EntityParameter();
                    id.ParameterName = "albumid";
                    id.Value = album.Id;
                    entityCommand.Parameters.Add(id);
                    entityCommand.CommandText = sql;

                    List<Track> tracks = null;
                    using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        while (dataReader.Read())
                        {
                            if (tracks == null)
                            {
                                tracks = new List<Track>();
                            }
                            Track track = new Track
                            {
                                Id = dataReader.GetInt32("LiedID", false, 0),
                                TrackNumber = dataReader.GetInt32("Track", false, 0),
                                Name = dataReader.GetString("Lied", false, string.Empty),
                                Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue)
                            };
                            tracks.Add(track);
                        }
                        if (tracks != null)
                        {
                            album.Tracks = tracks.ToArray();
                        }
                    }
                }
            }
        }
        private string GetTrackFilePath(IDataReader dataReader, string strAudioDirectory)
        {
            string strFilePath = null;
            if (dataReader != null)
            {
                string strPath = dataReader.GetString("Liedpfad", true, null);
                if (string.IsNullOrEmpty(strPath) == false)
                {
                    strFilePath = System.IO.Path.Combine(strAudioDirectory, strPath);
                }
            }
            return strFilePath;
        }
        
        
        #endregion
    }
}