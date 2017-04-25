﻿using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using BSE.Tunes.StoreApp.Models;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumsPageViewModel : SelectableItemsBaseViewModel
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<ListViewItemViewModel> m_albums;
        #endregion

        #region Properties
        public IncrementalObservableCollection<ListViewItemViewModel> Albums
        {
            get
            {
                return this.m_albums;
            }
            private set
            {
                this.m_albums = value;
                RaisePropertyChanged("Albums");
            }
        }
        #endregion

        #region MethodsPublic
        public async override void LoadData()
        {
            this.Albums = null;
            int iNumberOfPlayableAlbums = await DataService?.GetNumberOfPlayableAlbums();
            int pageNumber = 0;

            this.Albums = new IncrementalObservableCollection<ListViewItemViewModel>(
                (uint)iNumberOfPlayableAlbums,
                (uint count) =>
                {
                    Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                    {
                        int pageSize = (int)count;

                        ObservableCollection<Album> albums = await DataService?.GetAlbums(new Query
                        {
                            PageIndex = pageNumber,
                            PageSize = pageSize
                        });
                        if (albums != null)
                        {
                            foreach (var album in albums)
                            {
                                this.Albums.Add(new GridPanelItemViewModel
                                {
                                    Title = album.Title,
                                    Subtitle = album.Artist.Name,
                                    ImageSource = DataService?.GetImage(album.AlbumId, true),
                                    Data = album
                                });
                            }
                            pageNumber += pageSize;
                        }
                        return new Windows.UI.Xaml.Data.LoadMoreItemsResult()
                        {
                            Count = (uint)count
                        };
                    };
                    Task<Windows.UI.Xaml.Data.LoadMoreItemsResult> loadMoreItemsTask = taskFunc();
                    return loadMoreItemsTask.AsAsyncOperation<Windows.UI.Xaml.Data.LoadMoreItemsResult>();
                }
            );
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public async override void PlayAll(GridPanelItemViewModel item)
        {
            if (HasSelectedItems)
            {
                PlaySelectedItems();
            }
            else
            {
                Album album = item.Data as Album;
                if (album != null)
                {
                    album = await DataService.GetAlbumById(album.Id);
                    if (album.Tracks != null)
                    {
                        var trackIds = album.Tracks.Select(track => track.Id);
                        if (trackIds != null)
                        {
                            PlayerManager.PlayTracks(
                                new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                                PlayerMode.CD);
                        }
                    }
                }
            }
        }
        public async override void PlaySelectedItems()
        {
            var albumIds = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Album)itm.Data).Select(itm => itm.Id).ToList();
            if (albumIds != null)
            {
                var entryIds = await DataService.GetTrackIdsByAlbumIds(albumIds);
                if (entryIds != null)
                {
                    PlayerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                        PlayerMode.CD);
                }
            }
            ClearSelection();
        }
        #endregion
    }
}
