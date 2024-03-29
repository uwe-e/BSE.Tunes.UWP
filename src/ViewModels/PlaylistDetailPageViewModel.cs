﻿using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistDetailPageViewModel : PlaylistBaseViewModel
    {
        private Playlist m_playlist;
        private BitmapSource m_coverSource;
        private string m_subTitle;
        private ICommand m_showAlbumCommand;
        private RelayCommand m_playRandomCommand;
        private ICommand m_updatePlaylistCommand;
        private ICommand _removePlaylistCommand;

        public Playlist Playlist
        {
            get
            {
                return this.m_playlist;
            }
            set
            {
                this.m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
        public BitmapSource CoverSource
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
        public string InfoSubTitle
        {
            get
            {
                return this.m_subTitle;
            }
            set
            {
                this.m_subTitle = value;
                RaisePropertyChanged("InfoSubTitle");
            }
        }
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<ListViewItemViewModel>(ShowAlbum));
        public ICommand UpdatePlaylistCommand => m_updatePlaylistCommand ?? (m_updatePlaylistCommand = new RelayCommand(async () =>
        {
            await UpdatePlaylistEntries();
        }));
        public RelayCommand PlayRandomCommand => m_playRandomCommand ?? (m_playRandomCommand = new RelayCommand(PlayRandom, CanPlayRandom));

        public ICommand RemovePlaylistCommand => _removePlaylistCommand ?? (_removePlaylistCommand = new RelayCommand(async () =>
        {
            await RemovePlaylist();
        }));
        

        public PlaylistDetailPageViewModel()
        {
            Messenger.Default.Register<PlaylistChangedArgs>(this, args =>
            {
                if (args is PlaylistEntriesChangedArgs playlistEntriesChanged)
                {
                    LoadData(args.Playlist);
                }
            });
        }
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LoadData(parameter as Playlist);
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override bool CanPlayAll()
        {
            return this.Playlist?.Entries != null && this.Playlist.Entries.Count() > 0;
        }
        public override void PlayAll()
        {
            var entryIds = this.Playlist.Entries.Select(entry => entry.TrackId);
            if (entryIds != null)
            {
                PlayerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                    PlayerMode.Playlist);
            }
        }

        protected override void PlaySelectedItems()
        {
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                //var entries = selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<PlaylistEntry>();
                var entryIds = new System.Collections.ObjectModel.ObservableCollection<int>(selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<PlaylistEntry>().Select(p => p.TrackId));
                if (entryIds?.Count() > 0)
                {
                    PlayerManager.PlayTracks(entryIds, PlayerMode.Playlist);
                }
                this.SelectedItems.Clear();
            }
        }

        protected override void PlayNextItems()
        {
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var entryIds = new System.Collections.ObjectModel.ObservableCollection<int>(selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<PlaylistEntry>().Select(p => p.TrackId));
                if (entryIds?.Count() > 0)
                {
                    PlayerManager.InsertTracksToPlayQueue(entryIds, PlayerMode.Song);
                }
                this.SelectedItems.Clear();
            }
        }

        public override void PlayTrack(ListViewItemViewModel item)
        {
            //PlayerManager.PlayTrack(((PlaylistEntry)item.Data).TrackId, PlayerMode.Song);
            PlayerManager.PlayTracks(new ObservableCollection<int> { ((PlaylistEntry)item.Data).TrackId }, PlayerMode.Song);
        }

        public async override void DeleteSelectedItems()
        {
            if (SelectedItems?.Count > 0)
            {
                var list = SelectedItems.ToList();
                ;
                foreach (var item in list)
                {
                    Items.Remove((ListViewItemViewModel)item);
                }
                await UpdatePlaylistEntries();
                ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                await cacheableBitmapService.RemoveCache(Playlist.Guid.ToString());
                Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistEntriesChangedArgs(Playlist));
            }
        }

        protected override void AddAllToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var entries = new ObservableCollection<PlaylistEntry>(Playlist.Entries);
                if (entries?.Count() > 0)
                {
                    foreach (var entry in entries)
                    {
                        playlist.Entries.Add(entry);
                    }
                    AppendToPlaylist(playlist);
                }
            }
        }

        protected override void AddSelectedToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                if (this.SelectedItems is ObservableCollection<object> selectedItems)
                {
                    var entries = selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<PlaylistEntry>();
                    if (entries?.Count() > 0)
                    {
                        foreach (var entry in entries)
                        {
                            playlist.Entries.Add(entry);
                        }
                        AppendToPlaylist(playlist);
                    }
                    selectedItems.Clear();
                }
            }
        }

        protected override void AppendAllToPlayQueue()
        {
            if (Playlist?.Entries is IList<PlaylistEntry> entries)
            {
                var trackIds = new ObservableCollection<int>(entries.Select(p => p.TrackId));
                if (trackIds?.Count() > 0)
                {
                    PlayerManager.AppendTracksToPlayQueue(
                            new ObservableCollection<int>(trackIds),
                            PlayerMode.Song);

                }
            }
        }

        protected override void AppendSelectedToPlayQueue()
        {
            if (this.SelectedItems is ObservableCollection<object> selectedItems)
            {
                var trackIds = new ObservableCollection<int>(selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<PlaylistEntry>().Select(p => p.TrackId));
                if (trackIds?.Count() > 0)
                {
                    PlayerManager.AppendTracksToPlayQueue(
                            new ObservableCollection<int>(trackIds),
                            PlayerMode.Song);

                }
            }
            base.AppendSelectedToPlayQueue();
        }
        protected override void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnSelectedItemsCollectionChanged(sender, e);
            AllItemsSelected = Items.OrderBy(itm => ((PlaylistEntry)itm.Data).SortOrder).SequenceEqual(
                SelectedItems.Cast<ListViewItemViewModel>().OrderBy(itm => ((PlaylistEntry)itm.Data).SortOrder));
            AllItemsSelectable = HasSelectedItems & !AllItemsSelected;
        }

        private async void LoadData(Playlist playlist)
        {
            Items.Clear();
            if (playlist != null)
            {
                User user = SettingsService.Instance.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    try
                    {
                        Collection<Uri> imageUris = null;
                        ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                        Playlist = await this.DataService.GetPlaylistById(playlist.Id, user.UserName);
                        if (this.Playlist != null)
                        {
                            foreach (var entry in this.Playlist.Entries?.OrderBy(pe => pe.SortOrder))
                            {
                                if (entry != null)
                                {
                                    Items.Add(new ListViewItemViewModel { Data = entry });
                                    if (imageUris == null)
                                    {
                                        imageUris = new Collection<Uri>();
                                    }
                                    imageUris.Add(this.DataService.GetImage(entry.AlbumId));
                                }
                            }
                            if (imageUris != null)
                            {
                                this.CoverSource = await cacheableBitmapService.GetBitmapSource(
                                    new ObservableCollection<Uri>(imageUris.Take(4)),
                                    this.Playlist.Guid.ToString(),
                                    500);
                            }
                            this.InfoSubTitle = FormatNumberOfEntriesString(Playlist);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        this.PlayAllCommand.RaiseCanExecuteChanged();
                        this.PlayRandomCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private async Task UpdatePlaylistEntries()
        {
            Playlist.Entries.Clear();
            foreach (var entry in Items.Select(itm => itm.Data).Cast<PlaylistEntry>())
            {
                entry.SortOrder = Items.IndexOf(Items.Where(itm => ((PlaylistEntry)itm.Data).Id == entry.Id).FirstOrDefault());
                Playlist.Entries.Add(entry);
            }
            await DataService.UpdatePlaylistEntries(Playlist);
        }
        
        private async Task RemovePlaylist()
        {
            DeletePlaylistContentDialogViewModel deletePlaylistDialog = new DeletePlaylistContentDialogViewModel();
            deletePlaylistDialog.Playlists.Add(Playlist as Playlist);
            deletePlaylistDialog.DeleteInformation = string.Format(CultureInfo.InvariantCulture, ResourceService.GetString("DeletePlaylistContentDialog_TxtDeleteInformation"), Playlist.Name);
            IDialogService dialogService = DialogService.Instance;
            var dialogResult = await dialogService.ShowContentDialogAsync(deletePlaylistDialog);
            if (dialogResult == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                await NavigationService.NavigateAsync(typeof(Views.PlaylistsPage), removeLastBackStackEntry:true);
            }
        }
        
        private string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.Entries?.Count ?? 0;
            }
            return string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, ResourceService.GetString("PlaylistItem_PartNumberOfEntries", "Songs"));
        }

        private async void ShowAlbum(ListViewItemViewModel item)
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), ((Track)((PlaylistEntry)item.Data).Track).Album);
        }

        private bool CanPlayRandom()
        {
            return CanPlayAll();
        }

        private void PlayRandom()
        {
            var entryIds = this.Playlist.Entries.Select(entry => entry.TrackId);
            if (entryIds != null)
            {
                PlayerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(entryIds).ToRandomCollection(),
                    PlayerMode.Playlist);
            }
        }
    }
}
