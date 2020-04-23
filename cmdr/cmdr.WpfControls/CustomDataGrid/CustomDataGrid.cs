﻿using cmdr.WpfControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace cmdr.WpfControls.CustomDataGrid
{
    public class CustomDataGrid : DataGrid
    {
        static CustomDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGrid), new FrameworkPropertyMetadata(typeof(CustomDataGrid)));
        }

        #region clear_filtering

        /*
         * some notes:
         * 
          viewmodel
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor != null);
                    (TsiFileViewModel) (DeviceViewModel)

           views:
              TSIfileview = whole thing, (it calls MLV) 

              mappingListView - the grid and bottom buttons
              mappingeditor - right side
            */

        private Dictionary<string, TextBox> filtering_textBoxes;

        private void RememberTextbox(TextBox textBox, DataGridColumnHeader header)
        {
            // Try to get the property bound to the column.
            // This should be stored as datacontext.
            string columnName = header.DataContext != null ?
                                        header.DataContext.ToString() : "";

            TextBox filter;
            if (!filtering_textBoxes.TryGetValue(columnName, out filter))
            {
                filtering_textBoxes.Add(columnName, textBox);
            }

        }
        
        public void ClearFiltering()
        {
            var a = filtering_textBoxes;
           
            // clear the text boxes
            foreach (KeyValuePair<string, TextBox> entry in filtering_textBoxes)
            {
                String key = entry.Key;
                TextBox tb = entry.Value;
                String old_contents = tb.Text;

                tb.Text = "";
            }

            // also clear the internal filters
            foreach (KeyValuePair<string, ColumnFilter> entry in columnFilters)
            {
                String key = entry.Key;
                ColumnFilter filter = entry.Value;

                filter.FilterValue = "";
            }

            ApplyFilters();
        }


        #endregion
        /*
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var i = 9;
            var ee = e; ///.PropertyName;

        }
        
        public CustomDataGrid() {
            PropertyChanged += (s1, e1) => {
                var ss = s;
                var e2 = e1;
                var e3 = e2.PropertyName;

                var i = 9;
            };

           */

        public CustomDataGrid()
        {
            // Initialize lists
            filtering_textBoxes = new Dictionary<string, TextBox>();
            columnFilters = new Dictionary<string, ColumnFilter>();
            propertyCache = new Dictionary<string, PropertyInfo>();

            // Add a handler for all text changes
            AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged), false);
            // Add an event for Datacontext changes (to clear the cache)
            DataContextChanged += new DependencyPropertyChangedEventHandler(FilteringDataGrid_DataContextChanged);
        }

        /// <summary>
        /// Clear the property cache if the datacontext changes.
        /// This could indicate that an other type of object is bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilteringDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            //int i = ((ListCollectionView)(CollectionViewSource.GetDefaultView(ItemsSource))).Count;


            propertyCache.Clear();
            return;

            // Add option to clean filter when changing pages here
            //reapply filters here?
            bool clear_filters = true;
            clear_filters = false;
            if (clear_filters) {
                ClearFiltering();
            } else {
                ApplyFilters();
            }
        }

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            if (eventArgs.Column.SortDirection == null)
            {
                base.OnSorting(eventArgs);
            
            } else if (eventArgs.Column.SortDirection == System.ComponentModel.ListSortDirection.Ascending)
            {
                base.OnSorting(eventArgs);
 
            } else if (eventArgs.Column.SortDirection == System.ComponentModel.ListSortDirection.Descending)
            {
                // todo: make this an option using CTRL+mouse button
                bool allow_tri_state = false;

                if(allow_tri_state ){
                    eventArgs.Column.SortDirection = null;
                   Items.SortDescriptions.Clear();
                } else {
                    base.OnSorting(eventArgs);
                }
            } else {
                Console.WriteLine("Error - unknown sort order: ");

            }
        }

        #region Filtering

        // Filtering was made based on this article:
        // https://www.codeproject.com/Articles/41755/Filtering-the-WPF-DataGrid-automatically-via-the-h

        // This is the patch by TomWeps:
        // https://github.com/TakTraum/cmdr/compare/master...TomWeps:feature/filtering


        /// This dictionary will have a list of all applied filters
        private Dictionary<string, ColumnFilter> columnFilters;

        /// Cache with properties for better performance
        private Dictionary<string, PropertyInfo> propertyCache;

        // https://stackoverflow.com/questions/979876/set-background-color-of-wpf-textbox-in-c-sharp-code
        private void SetSearchBackground(TextBox t)
        {
            if(t.Text.Trim() == "") {
                var bc = new BrushConverter();
                t.Background = (Brush)bc.ConvertFrom("#FFF5F5F5");

            } else {
                //var prev = t.Background;
                t.Background = Brushes.Yellow;
            }
        }


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the textbox
            TextBox filterTextBox = e.OriginalSource as TextBox;

            // Get the header of the textbox
            DataGridColumnHeader header = TryFindParent<DataGridColumnHeader>(filterTextBox);
           
            if (header != null)
            {
                SetSearchBackground(filterTextBox);
                RememberTextbox(filterTextBox, header);
                UpdateFilter(filterTextBox, header);
                ApplyFilters();
            }

            e.Handled = true;
        }
        
        /// <summary>
        /// Update the internal filter
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="header"></param>
        private void UpdateFilter(TextBox textBox, DataGridColumnHeader header)
        {
            // Try to get the property bound to the column.
            // This should be stored as datacontext.
            string columnName = header.DataContext != null ?
                                        header.DataContext.ToString() : "";

            ColumnFilter filter;
            if(!columnFilters.TryGetValue(columnName, out filter))
            {
                // this code runs when we see a new filter
                var dataGridBoundColumn = header.Column as DataGridBoundColumn;
                if (dataGridBoundColumn != null)
                {
                    var binding = dataGridBoundColumn.Binding as Binding;
                    if(binding != null)
                    {
                        filter = new ColumnFilter
                        {
                            ColumnName = columnName,
                            ColumnBinding = binding
                        };

                        columnFilters.Add(filter.ColumnName, filter);
                    }
                }
            }

            if(filter!=null)
            {
                filter.FilterValue = textBox.Text;
            }           
        }
        

        /// <summary>
        /// Apply the filters
        /// </summary>
        /// <param name="border"></param>
        private void ApplyFilters()
        {

            // Get the view
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            if (view != null)
            {
                // Create a filter
                view.Filter = delegate (object item)
                {
                    var comparer = CultureInfo.CurrentCulture.CompareInfo;

                    // Show the current object
                    bool show = true;
                    // Loop filters
                    foreach (var filter in columnFilters.Values)
                    {
                        object property = GetPropertyValue(item as RowItemViewModel, filter.ColumnBinding);
                        if (property != null)
                        {
                            string propertyValue = property.ToString();
                            bool containsFilter = comparer.IndexOf(propertyValue, filter.FilterValue, CompareOptions.IgnoreCase) > -1;

                            // Do the necessary things if the filter is not correct
                            if (!containsFilter)
                            {
                                show = false;
                                break;
                            }

                        }
                    }
                    // Return if it's visible or not
                    return show;
                };
            }
        }

        private bool _changing_page = false;
        
        // This is how we link the mappings to the custom DataGrid
        // Because this came from XML
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // this is just for debugging
            //int c1 = ((ListCollectionView)(CollectionViewSource.GetDefaultView(ItemsSource))).Count;

            base.OnItemsChanged(e);

            // this is just for debugging
            //int c2 = ((ListCollectionView)(CollectionViewSource.GetDefaultView(ItemsSource))).Count;

            // Avoid infinite recursion here
            if (!_changing_page) {
                _changing_page = true;
                ApplyFilters();         //This is because triggering this triggers the event again
                _changing_page = false;

            }

            switch (e.Action) {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (RowItemViewModel item in e.NewItems)
                        item.ParentSelector = this;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    foreach (RowItemViewModel item in Items)
                        item.ParentSelector = this;
                    break;
                default:
                    break;
            }
        }

        private object GetPropertyValue(RowItemViewModel row, Binding binding)
        {
            object item = row.Item;
            string[] propertyNamePath = binding.Path.Path
                                    .Remove(0, "item.".Length)
                                    .Split('.');

            foreach(string name in propertyNamePath)
            {
                item = GetPropertyValue(item, name);
            }

            return item;
        }


        /// <summary>
        /// Get the value of a property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private object GetPropertyValue(object item, string property)
        {
            // No value
            object value = null;
            // Get property  from cache
            PropertyInfo pi = null;
            if (propertyCache.ContainsKey(property))
                pi = propertyCache[property];
            else
            {
                pi = item.GetType().GetProperty(property);
                propertyCache.Add(property, pi);
            }
            // If we have a valid property, get the value
            if (pi != null)
                value = pi.GetValue(item, null);
            // Done
            return value;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect
        /// child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found,
        /// a null reference is being returned.</returns>
        public static T TryFindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);
            //we've reached the end of the tree
            if (parentObject == null) return null;
            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }


        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Do note, that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;
                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }
            // If it's not a ContentElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        private class ColumnFilter
        {
            public string ColumnName { get; set; }
            public Binding ColumnBinding { get; set; }
            public string FilterValue { get; set; }
        }
       
        #endregion Filtering
    }
}


/*
 Improvements from the comments:
 

    ------
    Nice, but I didn't want the filter textbox to dissapear if it has the focus... Pin	Member	Mike Emerson	20-Feb-11 0:05 
upvote
downvote	
So I replaced the Trigger code in the ControlTemplate with

<MultiTrigger>
<MultiTrigger.Conditions>
<Condition Property="IsMouseOver" Value="False"/>
<Condition Property="IsFocused" SourceName="filterTextBox" Value="False"/>
</MultiTrigger.Conditions>
<MultiTrigger.ExitActions>
<BeginStoryboard x:Name="ShowFilterControl_BeginStoryboard" Storyboard="{StaticResource ShowFilterControl}"/>
<StopStoryboard BeginStoryboardName="HideFilterControl_BeginShowFilterControl"/>
</MultiTrigger.ExitActions>
<MultiTrigger.EnterActions>
<BeginStoryboard x:Name="HideFilterControl_BeginShowFilterControl" Storyboard="{StaticResource HideFilterControl}"/>
<StopStoryboard BeginStoryboardName="ShowFilterControl_BeginStoryboard"/>
</MultiTrigger.EnterActions>
</MultiTrigger>


Works like a charm.

    ------
    Go to ParentThe FilteringDataGrid is great, but I almost abandoned it because my datagrid is never bound to a simple list of properties. 
    Your input saved me. Just in case somebody needs C# code: here is mine (note that I renamed some member names to meet my standard):

Hide   Expand    Copy Code
private object GetPropertyValue(object item, string property)
{
    object value = null;
    PropertyInfo pi = null;

    if (property.Contains("."))
    {
        int n = property.IndexOf('.');
        var parent = property.Substring(0, n);
        if (_propertyCache.ContainsKey(parent))
            pi = _propertyCache[parent];
        else
        {
            pi = item.GetType().GetProperty(parent);
            _propertyCache.Add(parent, pi);
        }
        var child = pi.GetValue(item, null);
        if (child != null)
            value = GetPropertyValue(child, property.Substring(n + 1));
    }
    else
    {
        if (_propertyCache.ContainsKey(property))
            pi = _propertyCache[property];
        else
        {
            pi = item.GetType().GetProperty(property);
            _propertyCache.Add(property, pi);
        }
        if (pi != null)
            value = pi.GetValue(item, null);
    }
    return value;
}

 */
