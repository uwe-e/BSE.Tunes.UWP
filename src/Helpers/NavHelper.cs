using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Helpers
{
    public class NavHelper
    {
        // This helper class allows to specify the page that will be shown when you click on a NavigationViewItem
        //
        // Usage in xaml:
        // <winui:NavigationViewItem x:Uid="Shell_Main" Icon="Document" helpers:NavHelper.PageType="views:MainPage" />
        //
        // Usage in code:
        // NavHelper.SetPageType(navigationViewItem, typeof(MainPage));

        public static Type GetPageType(NavigationViewItem item)
        {
            return (Type)item.GetValue(PageTypeProperty);
        }

        public static void SetPageType(NavigationViewItem item, Type value)
        {
            item.SetValue(PageTypeProperty, value);
        }

        public static readonly DependencyProperty PageTypeProperty =
            DependencyProperty.RegisterAttached("PageType", typeof(Type), typeof(NavHelper), new PropertyMetadata(null));
    }
}
