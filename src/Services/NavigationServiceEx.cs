using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.Services
{
    public class NavigationServiceEx
    {
        // This NavigationService can handle navigation in an application that uses various frames.
        // The NavigationService manages a global backstack with entries from all frames.
        // Internally the NavigationService uses these frame keys to identify the frame in navigation and manage the navigation back stack.
        private const string FrameKeyMain = "Main";
        private const string FrameKeyShell = "ShellFrame";

        private Frame _frame;
        private bool _navigateFullscreen;
        private UIElement _shell;
        private static readonly Dictionary<string, Frame> _frames = new Dictionary<string, Frame>();

        public event Windows.UI.Xaml.Navigation.NavigatedEventHandler Navigated;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationFailedEventHandler NavigationFailed;

        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                _frame?.BackStack.Clear();
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public bool CanGoForward => Frame.CanGoForward;

        public bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                return true;
            }

            return false;
        }

        public void GoForward() => Frame.GoForward();

        public async Task<bool> NavigateAsync(Type page, object parameter = null, NavigationTransitionInfo infoOverride = null, bool navitageFullscreen = false)
        {
            if (navitageFullscreen && _navigateFullscreen != navitageFullscreen)
            {
                // the default frame should be the shell frame. This frame was created at startup
                _shell = _shell ?? Window.Current.Content;

                if (!_frames.ContainsKey(FrameKeyMain))
                {
                    _frames.Add(FrameKeyMain, new Frame());
                }

                Window.Current.Content = Frame = _frames.GetValueOrDefault(FrameKeyMain);

            }
            //if (!navitageFullscreen && _navigateFullscreen != navitageFullscreen)
            if (!navitageFullscreen)
            {
                // if the call comes from a fullscreen page
                if (_navigateFullscreen != navitageFullscreen)
                {
                    Window.Current.Content = _shell;
                    Frame = _frames.GetValueOrDefault(FrameKeyShell);
                }

                //If the first navigate call comes from the shellpage
                Frame = Frame ?? _frames.GetValueOrDefault(FrameKeyShell);
            }
            _navigateFullscreen = navitageFullscreen;


            var navigationHandled = new TaskCompletionSource<bool>();

            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                navigationHandled.SetResult(Frame.Navigate(page, parameter, infoOverride));
            });
            return await navigationHandled.Task;
        }

        public void InitializeShell(Frame shellFrame)
        {
            if (shellFrame != null)
            {
                _frames.Add(FrameKeyShell, shellFrame);
            }
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException(string.Format("The key {0} is already configured in NavigationService", key));
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException(string.Format("This type is already configured with key {0}", _pages.First(p => p.Value == pageType).Key));
                }

                _pages.Add(key, pageType);
            }
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            lock (_pages)
            {
                if (_pages.ContainsValue(page))
                {
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                }
                else
                {
                    //throw new ArgumentException(string.Format("The page '{0}' is unknown by the NavigationService", page.Name));
                    return "";
                }
            }
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating += Frame_Navigating;
                _frame.Navigated += Frame_Navigated;
                _frame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating -= Frame_Navigating;
                _frame.Navigated -= Frame_Navigated;
                _frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (sender is Frame frame)
            {
                if (frame.Content is Page page)
                {
                    if (page.DataContext is INavigable navigable)
                    {
                        navigable.OnNavigatedFromAsync(null, false);
                    }
                }
            }

            Navigating?.Invoke(sender, e);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                if (frame.Content is Page page)
                {
                    if (page.DataContext is INavigable navigable)
                    {
                        navigable.OnNavigatedToAsync(e.Parameter, e.NavigationMode, null);
                    }
                }
            }

            Navigated?.Invoke(sender, e);
        }
        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

    }
}
