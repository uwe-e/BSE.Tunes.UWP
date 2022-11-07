using BSE.Tunes.StoreApp.Controls.Extensions;
using System;
using System.Timers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls
{
    public class MenuFlyoutItem : Windows.UI.Xaml.Controls.MenuFlyoutItem
    {
        private DispatcherTimer _timer;

        public static readonly DependencyProperty IsIconEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsIconEnabled),
            typeof(bool),
            typeof(MenuFlyoutItem), new PropertyMetadata(false, OnIsIconEnabledChanged));

        public bool IsIconEnabled
        {
            get
            {
                return (bool)GetValue(IsIconEnabledProperty);
            }
            set
            {
                SetValue(IsIconEnabledProperty, value);
            }
        }

        public MenuFlyoutItem()
        {
            DefaultStyleKey = typeof(MenuFlyoutItem);
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = TimeSpan.FromTicks(1);
        }

        protected virtual void OnShowIconChanged(object oldValue, object newValue)
        {
            if (IsIconEnabled)
            {
                _timer.Start();
            }
        }

        private void OnTimerTick(object sender, object e)
        {
            if (IsIconEnabled)
            {
                VisualStateManager.GoToState(this, "IconEnabled", false);
            }
            _timer.Stop();
            _timer = null;
        }

        private static void OnIsIconEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuFlyoutItem)d).OnShowIconChanged(e.OldValue, e.NewValue);
        }
    }
}
