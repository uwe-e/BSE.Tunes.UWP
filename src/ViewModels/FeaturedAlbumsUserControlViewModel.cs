using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Models.Contract;
using System.Linq;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class FeaturedAlbumsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        public override async void LoadData()
        {
            var newestAlbums = await DataService.GetNewestAlbums(20);
            if (newestAlbums != null)
            {
                foreach (var album in newestAlbums)
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
        public override async void SelectItem(GridPanelItemViewModel item)
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public override async void NavigateTo()
        {
            await NavigationService.NavigateAsync(typeof(Views.AlbumsPage));
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
    }
}
