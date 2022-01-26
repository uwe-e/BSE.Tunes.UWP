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
        public event Windows.UI.Xaml.Navigation.NavigatedEventHandler Navigated;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationFailedEventHandler NavigationFailed;

        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;
        private object _lastParamUsed;

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

        public async Task<bool> NavigateAsync(Type page, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            //return await Task.Run(() => Navigate(page, parameter, infoOverride));
            //return await Task.Run(() => Frame.Navigate(page, parameter, infoOverride));
            //return Frame.Navigate(page, parameter, infoOverride);

            var navigationHandled = new TaskCompletionSource<bool>();

            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                navigationHandled.SetResult(Frame.Navigate(page, parameter, infoOverride));
            });
            return await navigationHandled.Task;
        }

        public bool Navigate(Type page, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            //return Navigate(page.FullName, parameter, infoOverride);
            //if (Frame.Content is Page page)
            //{

            //}
            return Frame.Navigate(page, parameter, infoOverride); ;
        }

        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            Type page;
            lock (_pages)
            {
                if (string.IsNullOrEmpty(pageKey) || !_pages.TryGetValue(pageKey, out page))
                {
                    throw new ArgumentException(string.Format("Invalid pageKey '{0}', please provide a valid pageKey. Maybe you forgot to call NavigationService.Configure?", pageKey), nameof(pageKey));
                }
            }

            if (Frame.Content?.GetType() != page || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var navigationResult = Frame.Navigate(page, parameter, infoOverride);
                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
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
