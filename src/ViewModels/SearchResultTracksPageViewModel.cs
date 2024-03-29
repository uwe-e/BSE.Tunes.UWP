﻿using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksPageViewModel : SelectableItemsBaseViewModel
    {
        private IncrementalObservableCollection<ListViewItemViewModel> m_tracks;
        private string m_headerText;
        private string m_pageHeaderText;
        private ICommand m_showAlbumCommand;
        private RelayCommand m_playNextItemsCommand;

        #region Properties
        public string HeaderText
        {
            get
            {
                return m_headerText;
            }
            set
            {
                m_headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }
        public string PageHeaderText
        {
            get
            {
                return m_pageHeaderText;
            }
            set
            {
                m_pageHeaderText = value;
                RaisePropertyChanged(() => PageHeaderText);
            }
        }
        public IncrementalObservableCollection<ListViewItemViewModel> Tracks
        {
            get
            {
                return this.m_tracks;
            }
            private set
            {
                this.m_tracks = value;
                RaisePropertyChanged(() => Tracks);
            }
        }
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<GridPanelItemViewModel>(ShowAlbum));

        public RelayCommand PlayNextItemsCommand => m_playNextItemsCommand ?? (m_playNextItemsCommand = new RelayCommand(PlayNextItems, CanPlayNextItems));

        

        #endregion

        #region MethodsPublic
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            if (parameter is Query query)
            {
                if (!string.IsNullOrEmpty(query.SearchPhrase))
                {
                    HeaderText = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", query.SearchPhrase);
                    PageHeaderText = string.Format(CultureInfo.CurrentUICulture, ResourceService.GetString("SearchResultTracksPage_PageHeaderText"), query.SearchPhrase);
                    LoadQueryResult(query);
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            PlayerManager.PlayTrack(((Track)item.Data).Id, PlayerMode.Song);
        }
        public override void PlayAll(GridPanelItemViewModel item)
        {
            if (HasSelectedItems)
            {
                PlaySelectedItems();
            }
            else
            {
                PlayerManager.PlayTrack(((Track)item.Data).Id, PlayerMode.Song);
            }
        }
        public override void PlaySelectedItems()
        {
            var trackIds = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Track)itm.Data).Select(itm => itm.Id).ToList();
            if (trackIds != null)
            {
                PlayerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                    PlayerMode.CD);
            }
            ClearSelection();
        }
        #endregion

        #region MethodsPrivate
        private void LoadQueryResult(Query query)
        {
            int maximumItems = 100;
            int pageIndex = 0;

            this.Tracks = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)maximumItems,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;

                            query.PageIndex = pageIndex;
                            query.PageSize = pageSize;

                            var tracks = await DataService.GetTrackSearchResults(query);
                            if (tracks != null)
                            {
                                foreach (var track in tracks)
                                {
                                    Tracks.Add(new GridPanelItemViewModel
                                    {
                                        Data = track
                                    });
                                }
                                pageIndex += pageSize;
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
        private async void ShowAlbum(GridPanelItemViewModel item)
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), (Album)((Track)item.Data).Album);
        }
        
        private bool CanPlayNextItems()
        {
            return SelectedItems?.Count > 0;
        }

        private void PlayNextItems()
        {
            var trackIds = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Track)itm.Data).Select(itm => itm.Id).ToList();
            if (trackIds != null)
            {
                PlayerManager.InsertTracksToPlayQueue(
                    new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                    PlayerMode.Song);
            }
            ClearSelection();
        }
        #endregion
    }
}
