using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls
{
    public class ListViewItemEx : ListViewItem
    {
        private const string _preSelectionCheckName = "PreSelectCheck";

        private FontIcon _preSelectCheck;

        public static readonly DependencyProperty ListViewProperty =
            DependencyProperty.Register(nameof(ListView), typeof(ListView), typeof(ListView), null);

        public ListView ListView
        {
            get { return (ListView)this.GetValue(ListViewProperty); }
            set { this.SetValue(ListViewProperty, value); }
        }

        /// <summary>
        /// Attached <see cref="nameof(EnablePreSelectionProperty)"/>
        /// </summary>
        public static readonly DependencyProperty EnablePreSelectionProperty =
            DependencyProperty.RegisterAttached(nameof(EnablePreSelection),
            typeof(bool),
            typeof(ListViewItemEx), new PropertyMetadata(false, EnablePreSelectionChanged));

        /// <summary>
        /// Gets or sets the <see cref="nameof(EnablePreSelection)"/> command
        /// </summary>
        public bool EnablePreSelection
        {
            get
            {
                return (bool)GetValue(EnablePreSelectionProperty);
            }
            set
            {
                SetValue(EnablePreSelectionProperty, value);
            }
        }
        
        private static void EnablePreSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is ListViewItemEx lvi)
            {
                if (lvi.EnablePreSelection)
                {
                    VisualStateManager.GoToState(lvi, "EnablePreSelection", true);
                }
                else
                {
                    VisualStateManager.GoToState(lvi, "DisablePreSelection", false);
                }
            }
        }
        
        public ListViewItemEx()
        {
            DefaultStyleKey = typeof(ListViewItemEx);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _preSelectCheck = this.GetTemplateChild(_preSelectionCheckName) as FontIcon;
            if (_preSelectCheck != null)
            {
                _preSelectCheck.Tapped += PreSelectCheckTapped;
                _preSelectCheck.PointerPressed += PreSelectCheckPointerPressed;
            }
        }

        private void PreSelectCheckPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void PreSelectCheckTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.ListView.SetPreselection(this.Content);
            e.Handled = true;
        }
    }
}
