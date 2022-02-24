using BSE.Tunes.StoreApp.ViewModels;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BSE.Tunes.StoreApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        //public static ShellPage Instance { get; set; }

        //public static WinUI.NavigationView NavigationView => Instance.navigationView;

        private ShellViewModel ViewModel
        {
            get { return ViewModelLocator.Current.ShellViewModel; }
        }

        public ShellPage()
        {

            this.InitializeComponent();
            //Instance = this;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);

            ViewModelLocator.Current.NavigationService.InitializeShell(shellFrame);

            Window.Current.SetTitleBar(AppTitleBar);

            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);

            navigationView.DisplayModeChanged += (sender, e) =>
            {
                Thickness currMargin = AppTitleBar.Margin;
                if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitleBar.Margin = new Thickness((sender.CompactPaneLength * 2), currMargin.Top, currMargin.Right, currMargin.Bottom);

                }
                else
                {
                    AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            };
        }

        private void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
            //coreTitleBar.ButtonBackgroundColor = Colors.Transparent;
            //coreTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            /*
             * WinUI:NavigationView does not navigate in Release Mode
             * https://github.com/microsoft/TemplateStudio/issues/2774
             */

            ViewModel.ItemInvokedCommand.Execute(args);
        }

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            /*
             * WinUI:NavigationView does not navigate in Release Mode
             * https://github.com/microsoft/TemplateStudio/issues/2774
             * 
             */

            ViewModel.BackRequestedCommand.Execute(args);
        }

    }
}
