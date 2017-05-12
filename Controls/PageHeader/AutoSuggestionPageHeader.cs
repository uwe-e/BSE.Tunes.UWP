﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls
{
    [TemplatePart(Name = PageHeaderName, Type = typeof(PageHeader))]
    [TemplatePart(Name = AutoSuggestBoxName, Type = typeof(AutoSuggestBox))]
    [TemplatePart(Name = AppBarButtonName, Type = typeof(AppBarButton))]
    public partial class AutoSuggestionPageHeader : Control
    {
        private const string PageHeaderName = "PART_PageHeader";
        private const string AutoSuggestBoxName = "PART_AutoSuggestBox";
        private const string AppBarButtonName = "PART_AppBarButton";

        private PageHeader m_pageHeader;
        private AutoSuggestBox m_autoSuggestBox;
        private AppBarButton m_appBarButton;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;

        //public static readonly DependencyProperty QueryIconProperty =
        //    DependencyProperty.Register("QueryIcon", typeof(IconElement), typeof(AutoSuggestionPageHeader),
        //        new PropertyMetadata(null, OnQueryIconChanged));

        public static readonly DependencyProperty QueryIconProperty =
            DependencyProperty.Register("QueryIcon", typeof(IconElement), typeof(AutoSuggestionPageHeader),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonIconProperty =
           DependencyProperty.Register("ButtonIcon", typeof(IconElement), typeof(AutoSuggestionPageHeader),
               new PropertyMetadata(null));

        public static readonly DependencyProperty QueryTextProperty =
            DependencyProperty.Register("QueryText", typeof(string), typeof(AutoSuggestionPageHeader),
                new PropertyMetadata(default(string), OnQueryTextChanged));

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(AutoSuggestionPageHeader),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(AutoSuggestionPageHeader),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(object),
                typeof(AutoSuggestionPageHeader), new PropertyMetadata(null, OnItemsSourceChanged));

        public IconElement QueryIcon
        {
            get
            {
                return (IconElement)GetValue(QueryIconProperty);
            }
            set
            {
                SetValue(QueryIconProperty, value);
            }
        }

        public IconElement ButtonIcon
        {
            get
            {
                return (IconElement)GetValue(ButtonIconProperty);
            }
            set
            {
                SetValue(ButtonIconProperty, value);
            }
        }

        public string QueryText
        {
            get
            {
                return (string)GetValue(QueryTextProperty);
            }
            set
            {
                SetValue(QueryTextProperty, value);
            }
        }

        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }

        public string PlaceholderText
        {
            get
            {
                return (string)GetValue(PlaceholderTextProperty);
            }
            set
            {
                SetValue(PlaceholderTextProperty, value);
            }
        }

        public object ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public AutoSuggestionPageHeader()
        {
            this.DefaultStyleKey = typeof(AutoSuggestionPageHeader);
        }
        protected override void OnApplyTemplate()
        {
            m_pageHeader = base.GetTemplateChild(PageHeaderName) as PageHeader;
            m_pageHeader?.RegisterPropertyChangedCallback(ContentControl.ContentProperty, OnPageHeaderContentChanged);

            m_autoSuggestBox = base.GetTemplateChild(AutoSuggestBoxName) as AutoSuggestBox;
            if (m_autoSuggestBox != null)
            {
                m_autoSuggestBox.QuerySubmitted += (sender, args) =>
                {
                    if (QuerySubmitted != null)
                    {
                        QuerySubmitted(sender, args);
                    }
                };
                m_autoSuggestBox.SuggestionChosen += (sender, args) =>
                {
                    if (SuggestionChosen != null)
                    {
                        SuggestionChosen(sender, args);
                    }
                };
                m_autoSuggestBox.TextChanged += (sender, args) =>
                {
                    if (TextChanged != null)
                    {
                        TextChanged(sender, args);
                    }
                };
            }

            m_appBarButton = base.GetTemplateChild(AppBarButtonName) as AppBarButton;

            //OnQueryIconChanged(this);
            //OnButtonIconChanged(this);
            //OnQueryTextChanged(this);
            OnHeaderTextChanged(this);
            //OnPlaceholderTextChanged(this);
            OnItemsSourceChanged(this);

            base.OnApplyTemplate();
        }

        //protected virtual void OnQueryIconChanged(DependencyObject dependencyObject)
        //{
        //    if (m_autoSuggestBox != null)
        //    {
        //        m_autoSuggestBox.QueryIcon = QueryIcon;
        //    }
        //}

        //protected virtual void OnButtonIconChanged(DependencyObject dependencyObject)
        //{
        //    if (m_appBarButton != null)
        //    {
        //        m_appBarButton.Icon = ButtonIcon;
        //    }
        //}

        //protected virtual void OnQueryTextChanged(DependencyObject dependencyObject)
        //{
        //    if (m_autoSuggestBox != null)
        //    {
        //        m_autoSuggestBox.Text = QueryText;
        //    }
        //}

        protected virtual void OnHeaderTextChanged(DependencyObject dependencyObject)
        {
            if (m_pageHeader != null)
            {
                m_pageHeader.Content = HeaderText;
            }
        }
        protected virtual void OnQueryTextChanged(object oldValue, object newValue)
        {
            //var value = oldValue as INotifyCollectionChanged;
        }
        //protected virtual void OnPlaceholderTextChanged(DependencyObject dependencyObject)
        //{
        //    if (m_autoSuggestBox != null)
        //    {
        //        m_autoSuggestBox.PlaceholderText = PlaceholderText;
        //    }
        //}

        protected virtual void OnItemsSourceChanged(DependencyObject dependencyObject)
        {
            if (m_autoSuggestBox != null)
            {
                m_autoSuggestBox.ItemsSource = ItemsSource;
            }
        }

        private async void OnPageHeaderContentChanged(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            var pageHeader = (PageHeader)dependencyObject;
            var autoSuggestBox = pageHeader.Content as AutoSuggestBox;
            if (autoSuggestBox != null)
            {
                // have to wait until the AutoSuggestBox apeared on the screen as the PageHeader's content
                await Task.Yield();
                autoSuggestBox.Focus(FocusState.Programmatic);
            }
        }

        private static void OnQueryIconChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //((AutoSuggestionPageHeader)dependencyObject).OnQueryIconChanged(dependencyObject);
        }

        private static void OnButtonIconChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //((AutoSuggestionPageHeader)dependencyObject).OnButtonIconChanged(dependencyObject);
        }

        private static void OnQueryTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((AutoSuggestionPageHeader)dependencyObject).OnQueryTextChanged(dependencyPropertyChangedEventArgs.OldValue, dependencyPropertyChangedEventArgs.NewValue);
            //((AutoSuggestionPageHeader)dependencyObject).OnQueryTextChanged(dependencyObject);
        }

        private static void OnHeaderTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((AutoSuggestionPageHeader)dependencyObject).OnHeaderTextChanged(dependencyObject);
        }

        private static void OnPlaceholderTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //((AutoSuggestionPageHeader)dependencyObject).OnPlaceholderTextChanged(dependencyObject);
        }
        private static void OnItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((AutoSuggestionPageHeader)dependencyObject).OnItemsSourceChanged(dependencyObject);
        }
    }
}