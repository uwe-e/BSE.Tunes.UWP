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
        /// Attached Command Property
        /// </summary>
        public static readonly DependencyProperty PreSelectionCommandProperty =
            DependencyProperty.RegisterAttached(nameof(PreSelectionCommand),
            typeof(ICommand),
            typeof(ListView),
            new PropertyMetadata(null, OnCommandChanged));

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

        ///// <summary>
        ///// Gets the command to invoke when an item is clicked.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns>The invoked command.</returns>
        //public static ICommand GetPreSelectionCommand(DependencyObject obj)
        //{
        //    return (ICommand)obj.GetValue(PreSelectionCommandProperty);
        //}
        ///// <summary>
        ///// Sets the command to invoke when an item is clicked.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="value">The command.</param>
        //public static void SetPreSelectionCommand(DependencyObject obj, ICommand value)
        //{
        //    obj.SetValue(PreSelectionCommandProperty, value);
        //}
        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListView;
            if (control != null)
            {
                control.ItemPreSelectionClick += OnItemPreSelectionClick;
            }
        }

        private static void OnItemPreSelectionClick(object sender, ItemPreSelectionClickEventArgs e)
        {
            //throw new NotImplementedException();
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
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void SetPreselection(object SelectedItem)
        {
            object selectedItem = SelectedItem;
            if (selectedItem != null)
            {
                //this.SelectedItems.Add(selectedItem);
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
            ListView control = dependencyObject as ListView;
            if (control != null)
            {
                control.AlternatingRow = dependencyPropertyChangedEventArgs.NewValue as Brush;
            }
        }
    }
}
