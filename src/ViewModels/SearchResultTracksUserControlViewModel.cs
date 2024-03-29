﻿using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksUserControlViewModel : FeaturedItemsBaseViewModel
    {
        private Query m_query;
        private ICommand m_showAlbumCommand;
        private ICommand m_playNextItemsCommand;

        #region Properties
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
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<GridPanelItemViewModel>(ShowAlbum));

        public ICommand PlayNextItemsCommand => m_playNextItemsCommand ?? (m_playNextItemsCommand = new RelayCommand<GridPanelItemViewModel>(PlayNextItem));
        #endregion

        #region MethodsPublic
        public SearchResultTracksUserControlViewModel()
        {
        }
        public SearchResultTracksUserControlViewModel(Query query) : base()
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
                var tracks = await DataService.GetTrackSearchResults(Query);
                if (tracks != null)
                {
                    foreach (var track in tracks)
                    {
                        if (track != null)
                        {
                            Items.Add(new GridPanelItemViewModel
                            {
                                Data = track
                            });
                        }
                    }
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            PlayerManager.PlayTrack(((Track)item.Data).Id, PlayerMode.Song);
        }

        private void PlayNextItem(GridPanelItemViewModel item)
        {
            if (item?.Data is Track track)
            {
                PlayerManager.InsertTracksToPlayQueue(
                    new ObservableCollection<int> { track.Id }, PlayerMode.Song);
            }
        }

        public override async void NavigateTo()
        {
            await NavigationService.NavigateAsync(typeof(Views.SearchResultTracksPage), Query);
        }
        #endregion

        #region MethodsPrivate
        private async void ShowAlbum(GridPanelItemViewModel item)
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), (Album)((Track)item.Data).Album);
        }
        #endregion
    }
}
