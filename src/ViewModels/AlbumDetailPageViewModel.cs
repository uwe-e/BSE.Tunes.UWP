﻿using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumDetailPageViewModel : PlaylistBaseViewModel
    {
        private Album m_album;
        private Uri m_coverSource;
        private ArtistsAlbumsUserControlViewModel m_artistsAlbums;

        private RelayCommand<object> m_playTrackCommand1;

        public RelayCommand<object> PlayTrackCommand1 => m_playTrackCommand1 ?? (m_playTrackCommand1 = new RelayCommand<object>( (o) =>
        {
            if (o is ListViewItemViewModel item)
            {
                SelectedItems.Add(item as ListViewItemViewModel);
            }
            //IList<object> items = o as IList<object>;
            //if (items == null)
            //{
            //    SelectedItems.Clear();
            //}
            //else
            //{
            //    foreach (var item in items)
            //    {
            //        SelectedItems.Add(item as ListViewItemViewModel);
            //    }
            //    //SelectedItems = items;
            //}
            //var t = o;
        }));

        public Album Album
        {
            get
            {
                return this.m_album;
            }
            set
            {
                this.m_album = value;
                RaisePropertyChanged("Album");
            }
        }
        public Uri CoverSource
        {
            get
            {
                return this.m_coverSource;
            }
            set
            {
                this.m_coverSource = value;
                RaisePropertyChanged("CoverSource");
            }
        }
        public ArtistsAlbumsUserControlViewModel ArtistsAlbums
        {
            get
            {
                return m_artistsAlbums;
            }
            set
            {
                m_artistsAlbums = value;
                RaisePropertyChanged(() => ArtistsAlbums);
            }
        }

        public AlbumDetailPageViewModel()
        {
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            Album album = parameter as Album;
            if (album != null)
            {
                Album = album;
                Album = await DataService.GetAlbumById(album.Id);
                this.CoverSource = DataService.GetImage(album.AlbumId);
                
                foreach (Track track in Album.Tracks)
                {
                    if (track != null)
                    {
                        this.Items.Add(new ListViewItemViewModel { Data = track });
                    }
                }
                
                ArtistsAlbums = new ArtistsAlbumsUserControlViewModel(Album.Artist);
                this.PlayAllCommand.RaiseCanExecuteChanged();
            }
        }
        public override bool CanPlayAll()
        {
            return this.Album != null && this.Album.Tracks != null && this.Album?.Tracks?.Count() > 0;
        }
        public override void PlayAll()
        {
            var tracks = new System.Collections.ObjectModel.ObservableCollection<Track>(this.Album.Tracks);
            if (tracks != null && tracks.Count() > 0)
            {
                this.PlayTracks(tracks);
            }
        }

        protected override void PlaySelectedItems()
        {
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var selectedTracks = new System.Collections.ObjectModel.ObservableCollection<Track>(selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<Track>());
                if (selectedTracks?.Count() > 0)
                {
                    this.PlayTracks(selectedTracks);
                }
                this.SelectedItems.Clear();
            }
        }
        protected override void AddAllToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var tracks = new ObservableCollection<Track>(this.Album.Tracks);
                if (tracks != null)
                {
                    this.AddTracksToPlaylist(playlist, tracks);
                }
            }
        }
        protected override void AddSelectedToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var selectedItems = this.SelectedItems;
                if (selectedItems != null)
                {
                    var tracks = new ObservableCollection<Track>(selectedItems.Cast<ListViewItemViewModel>()
                        .Select(itm => itm.Data).Cast<Track>());
                    if (tracks != null)
                    {
                        this.AddTracksToPlaylist(playlist, tracks);
                    }
                }
            }
            this.SelectedItems?.Clear();
        }
        protected override void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnSelectedItemsCollectionChanged(sender, e);
            AllItemsSelected = Items.OrderBy(itm => ((Track)itm.Data).TrackNumber).SequenceEqual(
                SelectedItems.Cast<ListViewItemViewModel>().OrderBy(itm => ((Track)itm.Data).TrackNumber));
            AllItemsSelectable = HasSelectedItems & !AllItemsSelected;
        }

        private void PlayTracks(System.Collections.ObjectModel.ObservableCollection<Track> tracks)
        {
            if (tracks != null)
            {
                var trackIds = tracks.Select(track => track.Id);
                if (trackIds != null)
                {
                    PlayerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                        PlayerMode.CD);
                }
            }
        }
        private void AddTracksToPlaylist(Playlist playlist, ObservableCollection<Track> tracks)
        {
            if (playlist != null && tracks != null)
            {
                foreach (var track in tracks)
                {
                    if (track != null)
                    {
                        playlist.Entries.Add(new PlaylistEntry
                        {
                            PlaylistId = playlist.Id,
                            TrackId = track.Id,
                            Guid = Guid.NewGuid()
                        });
                    }
                }
                this.AppendToPlaylist(playlist);
            }
        }
    }
}
