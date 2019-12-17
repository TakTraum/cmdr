using cmdr.WpfControls.ViewModels;
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

namespace cmdr.WpfControls.CustomDataGrid
{
    public class CustomDataGrid : DataGrid
    {
        static CustomDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGrid), new FrameworkPropertyMetadata(typeof(CustomDataGrid)));
        }


        public CustomDataGrid()
        {
            // Initialize lists
            columnFilters = new Dictionary<string, ColumnFilter>();
            propertyCache = new Dictionary<string, PropertyInfo>();
            // Add a handler for all text changes
            AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged), false);
            // Datacontext changed, so clear the cache
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
            propertyCache.Clear();
        }



        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
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

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            // Order is: None->Ascending->Descending->None
            if (eventArgs.Column.SortDirection != System.ComponentModel.ListSortDirection.Descending)
                base.OnSorting(eventArgs);
            else
            {
                eventArgs.Column.SortDirection = null;
                Items.SortDescriptions.Clear();
            }
        }

        #region Filtering

        // Filtering was made based on this article:
        // https://www.codeproject.com/Articles/41755/Filtering-the-WPF-DataGrid-automatically-via-the-h

        /// <summary>
        /// This dictionary will have a list of all applied filters
        /// </summary>
        private Dictionary<string, ColumnFilter> columnFilters;
        /// <summary>
        /// Cache with properties for better performance
        /// </summary>
        private Dictionary<string, PropertyInfo> propertyCache;


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the textbox
            TextBox filterTextBox = e.OriginalSource as TextBox;
            // Get the header of the textbox
            DataGridColumnHeader header = TryFindParent<DataGridColumnHeader>(filterTextBox);
           
            if (header != null)
            {
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
