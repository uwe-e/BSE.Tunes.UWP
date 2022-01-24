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
        // <winui:NavigationViewItem x:Uid="Shell_Main" Icon="Document" helpers:NavHelper.NavigateTo="AppName.ViewModels.MainViewModel" />
        //
        // Usage in code:
        // NavHelper.SetNavigateTo(navigationViewItem, typeof(MainViewModel).FullName);
        public static string GetNavigateTo(NavigationViewItem item)
        {
            return (string)item.GetValue(NavigateToProperty);
        }

        public static void SetNavigateTo(NavigationViewItem item, string value)
        {
            item.SetValue(NavigateToProperty, value);
        }

        public static readonly DependencyProperty NavigateToProperty =
            DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(NavHelper), new PropertyMetadata(null));


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
