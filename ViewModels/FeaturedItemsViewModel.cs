﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class FeaturedItemsViewModel: FeaturedItemsBaseViewModel
    {
        #region MethodsPublic
        public FeaturedItemsViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadData();
            }
        }
        #endregion

        #region MethodsPrivate
        public override async void LoadData()
        {
            this.ItemsGroup = new ItemsGroupViewModel();
            var newestAlbums = await DataService.GetFeaturedAlbums(6);
            if (newestAlbums != null)
            {
                foreach (var album in newestAlbums)
                {
                    if (album != null)
                    {
                        this.ItemsGroup.Items.Add(new ItemViewModel
                        {
                            Title = album.Title,
                            Subtitle = album.Artist.Name,
                            Data = album,
                            ImageSource = DataService.GetImage(album.AlbumId)
                        });
                    }
                }
            }
        }
        public override void SelectItem(ItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        #endregion
    }
}
