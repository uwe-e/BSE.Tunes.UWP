﻿using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Managers
{
    public class PlayerManager : IPlayerManager
    {
        private readonly IDataService m_dataService;
        private readonly IDialogService m_dialogService;
        private NavigableCollection<int> _playlist;

        public event NotifyCollectionChangedEventHandler PlaylistCollectionChanged;

        public IPlayerService PlayerService
        {
            get;
            private set;
        }

        public NavigableCollection<int> Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                if(_playlist != null)
                {
                    _playlist.CollectionChanged -= OnPlaylistCollectionChanged;
                }
                
                _playlist = value;
                _playlist.CollectionChanged += OnPlaylistCollectionChanged;
            }
        }

        private void OnPlaylistCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            NotifyCollectionChangedEventHandler handler = PlaylistCollectionChanged;
            handler?.Invoke(this, e);
        }

        public Track CurrentTrack
        {
            get
            {
                return this.PlayerService.CurrentTrack;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return this.PlayerService.Duration;
            }
        }

        public TimeSpan Position
        {
            get
            {
                return this.PlayerService.Position;
            }
        }

        public PlayerMode PlayerMode
        {
            get;
            private set;
        }

        public static PlayerManager Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayerManager>() as PlayerManager;
            }
        }

        public PlayerManager(IDataService dataService, IAuthenticationService accountService, IPlayerService playerService, IDialogService dialogservice, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.PlayerService = playerService;
            this.m_dialogService = dialogservice;
            Messenger.Default.Register<MediaStateChangedArgs>(this, args =>
            {
                switch (args.MediaState)
                {
                    case MediaState.Opened:
                        OnMediaOpened();
                        break;
                    case MediaState.Ended:
                        this.OnMediaEnded();
                        break;
                    case MediaState.NextRequested:
                        ExecuteNextTrack();
                        break;
                    case MediaState.PreviousRequested:
                        ExecutePreviousTrack();
                        break;
                    //case MediaState.DownloadCompleted:
                    //    PrepareNextTrack();
                    //    break;
                }
            });
        }

        public async void ReplayPlayTracks()
        {
            int trackId = this.Playlist?.FirstOrDefault() ?? 0;
            if (trackId > 0)
            {
                Track track = await this.m_dataService.GetTrackById(trackId);
                if (track != null)
                {
                    await SetTrackAsync(track);
                }
            }
        }

        public void PlayTracks(ObservableCollection<int> trackIds, PlayerMode playerMode)
        {
            this.Playlist = trackIds?.ToNavigableCollection();
            PlayTrack(this.Playlist?.FirstOrDefault() ?? 0, playerMode);
        }

        public async void PlayTrack(int trackId, PlayerMode playerMode)
        {
            this.PlayerMode = playerMode;
            if (trackId > 0)
            {
                var track = await this.m_dataService.GetTrackById(trackId);
                if (track != null)
                {
                    await this.SetTrackAsync(track);
                }
            }
        }
        public async Task SetTrackAsync(Track track)
        {
            if (track != null)
            {
                try
                {
                    await this.PlayerService.SetTrackAsync(track);
                }
                catch (Exception exception)
                {
                    await this.m_dialogService.ShowMessageDialogAsync(exception.Message);
                }
            }
        }

        public bool CanExecuteNextTrack()
        {
            return this.Playlist?.CanMoveNext ?? false;
        }

        public async void ExecuteNextTrack()
        {
            if (this.CanExecuteNextTrack())
            {
                if (this.Playlist.MoveNext())
                {
                    var trackId = this.Playlist.Current;
                    if (trackId > 0)
                    {
                        Track track = await this.m_dataService.GetTrackById(trackId);
                        if (track != null)
                        {
                            await this.SetTrackAsync(track);
                        }
                    }
                }
            }
        }
        public bool CanExecutePreviousTrack()
        {
            return this.Playlist?.CanMovePrevious ?? false;
        }
        public async void ExecutePreviousTrack()
        {
            if (this.CanExecutePreviousTrack())
            {
                if (this.Playlist.MovePrevious())
                {
                    var trackId = this.Playlist.Current;
                    if (trackId > 0)
                    {
                        var track = await this.m_dataService.GetTrackById(trackId);
                        if (track != null)
                        {
                            await this.SetTrackAsync(track);
                        }
                    }
                }
            }
        }

        //public async void PrepareNextTrack()
        //{
        //    if (this.CanExecuteNextTrack())
        //    {
        //        var trackId = this.Playlist[Playlist.Index + 1];
        //        if (trackId > 0)
        //        {
        //            var track = await this.m_dataService.GetTrackById(trackId);
        //            if (track != null)
        //            {
        //                await PlayerService.PrepareTrack(track);
        //                //await this.SetTrackAsync(track);
        //            }
        //        }
        //    }
        //}
        
        public void InsertTracksToPlayQueue(ObservableCollection<int> trackIds, PlayerMode playerMode)
        {
            var trackId = this.Playlist.Current;
            int index =  this.Playlist.IndexOf(trackId);
            foreach (int id in trackIds)
            {
                this.Playlist.Insert(index += 1, id);
            }
        }

        public void AppendTracksToPlayQueue(ObservableCollection<int> trackIds, PlayerMode playerMode)
        {
            if (trackIds is ObservableCollection<int> ids)
            {
                foreach (int id in ids)
                {
                    this.Playlist.Add(id);
                }
            }
        }

        public bool CanExecutePlay()
        {
            return this.Playlist?.Count > 0;
        }
        public void Play()
        {
            this.PlayerService.Play();
        }
        public void Pause()
        {
            this.PlayerService.Pause();
        }

        private void OnMediaEnded()
        {
            //if (this.PlayerMode != PlayerMode.None && this.PlayerMode != PlayerMode.Song)
            if (this.PlayerMode != PlayerMode.None)
            {
                this.ExecuteNextTrack();
            }
        }
        private void OnMediaOpened()
        {
            try
            {
                PlayerService.CanExecuteNextTrack = this.CanExecuteNextTrack();
                PlayerService.CanExecutePreviousTrack = this.CanExecutePreviousTrack();

                string userName = "unknown";
                User user = SettingsService.Instance.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    userName = user.UserName;
                }

                this.m_dataService.UpdateHistory(new History
                {
                    PlayMode = (int)this.PlayerMode,
                    AlbumId = this.CurrentTrack.Album.Id,
                    TrackId = this.CurrentTrack.Id,
                    UserName = userName,
                    PlayedAt = DateTime.Now
                });
            }
            catch { }
        }

        
    }
}
