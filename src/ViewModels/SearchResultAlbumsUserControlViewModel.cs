﻿using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using System.Linq;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultAlbumsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        private Query m_query;

        public Query Query
        {
            get
            {
                return m_query;
            }
            set
            {
                m_query = value;
                RaisePropertyChanged(() => Query);
            }
        }
        public SearchResultAlbumsUserControlViewModel()
        {
        }
        public SearchResultAlbumsUserControlViewModel(Query query) : base()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Query = query;
                LoadData();
            }
        }
        public async override void LoadData()
        {
            if (Query != null)
            {
                var albums = await DataService.GetAlbumSearchResults(Query);
                if (albums != null)
                {
                    foreach (var album in albums)
                    {
                        if (album != null)
                        {
                            Items.Add(new GridPanelItemViewModel
                            {
                                Title = album.Title,
                                Subtitle = album.Artist.Name,
                                Data = album,
                                ImageSource = DataService.GetImage(album.AlbumId, true)
                            });
                        }
                    }
                }
            }
        }
        public override async void SelectItem(GridPanelItemViewModel item)
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public override async void PlayAll(GridPanelItemViewModel item)
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
        public override async void NavigateTo()
        {
            await NavigationService.NavigateAsync(typeof(Views.SearchResultAlbumsPage), Query);
        }
    }
}
