using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BSE.Tunes.StoreApp.Controls
{
    public class ListView : Windows.UI.Xaml.Controls.ListView
    {
        public event EventHandler<ItemPreSelectionClickEventArgs> ItemPreSelectionClick;

        public static readonly DependencyProperty AlternatingRowProperty =
            DependencyProperty.Register("AlternatingRow", typeof(Brush), typeof(ListView),
                new PropertyMetadata(null, AlternatingRowChanged));

        public Brush AlternatingRow
        {
            get
            {
                return (Brush)GetValue(AlternatingRowProperty);
            }
            set
            {
                SetValue(AlternatingRowProperty, value);
            }
        }

        /// <summary>
        /// Attached <see cref="nameof(EnablePreSelectionProperty)"/>
        /// </summary>
        public static readonly DependencyProperty EnablePreSelectionProperty =
            DependencyProperty.RegisterAttached(nameof(EnablePreSelection),
            typeof(bool),
            typeof(ListView), new PropertyMetadata(false, OnEnablePreSelectionChanged));

        private static void OnEnablePreSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.ContainerContentChanging += OnListContainerContentChanging;
            }
        }

        private static void OnListContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemContainer is ListViewItemEx itemContainer)
            {
                itemContainer.EnablePreSelection = ((ListView)sender).EnablePreSelection;
            }
        }

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

        /// <summary>
        /// Attached <see cref="nameof(PreSelectionCommandProperty)"/>
        /// </summary>
        public static readonly DependencyProperty PreSelectionCommandProperty =
            DependencyProperty.RegisterAttached(nameof(PreSelectionCommand),
            typeof(ICommand),
            typeof(ListView), null);

        /// <summary>
        /// Gets or sets the <see cref="nameof(PreSelectionCommand)"/> command
        /// </summary>
        public ICommand PreSelectionCommand
        {
            get
            {
                return (ICommand)GetValue(PreSelectionCommandProperty);
            }
            set
            {
                SetValue(PreSelectionCommandProperty, value);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (AlternatingRow != null)
            {
                foreach (var itm in Items)
                {
                    var index = Items.IndexOf(itm);
                    ListViewItem lvi = this.ContainerFromIndex(index) as ListViewItem;
                    SetAlternatingBackground(lvi, index);
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItemEx();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            int index = IndexFromContainer(element);
            SetAlternatingBackground(element, index);

            if (element is ListViewItemEx lvi)
            {
                lvi.ListView = this;
            }
        }

        protected virtual void OnItemPreSelectionClick(ItemPreSelectionClickEventArgs e)
        {
            PreSelectionCommand?.Execute(e.SelectedItem);
            EventHandler<ItemPreSelectionClickEventArgs> handler = ItemPreSelectionClick;
            handler?.Invoke(this, e);
        }

        internal void SetPreselection(object SelectedItem)
        {
            object selectedItem = SelectedItem;
            if (selectedItem != null)
            {
                OnItemPreSelectionClick(new ItemPreSelectionClickEventArgs
                {
                    Source = this,
                    SelectedItem = selectedItem
                });
            }
        }

        private void SetAlternatingBackground(DependencyObject element, int index)
        {
            if (AlternatingRow != null)
            {
                ListViewItem lvi = element as ListViewItem;
                if (index % 2 == 0)
                {
                    lvi.Background = AlternatingRow;
                }
                else
                {
                    lvi.Background = Background;
                }
            }
        }

        private static void AlternatingRowChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ListView listView)
            {
                listView.AlternatingRow = dependencyPropertyChangedEventArgs.NewValue as Brush;
            }
        }
    }
}
