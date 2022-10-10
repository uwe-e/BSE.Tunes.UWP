using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls
{
    public class ListViewItemEx : ListViewItem
    {
        private const string _preSelectionCheckName = "PreSelectCheck";
        private const string _preSelectionSquareName = "PreSelectionSquare";

        private FontIcon _preSelectCheck;
        private Border __preSelectionSquare;

        public static readonly DependencyProperty ListViewProperty =
            DependencyProperty.Register(nameof(ListView), typeof(ListView), typeof(ListView), null);

        public ListView ListView
        {
            get { return (ListView)this.GetValue(ListViewProperty); }
            set { this.SetValue(ListViewProperty, value); }
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
