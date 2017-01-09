using cmdr.WpfControls.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace cmdr.WpfControls.CustomDataGrid
{
    public class CustomDataGrid : DataGrid
    {
        static CustomDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGrid), new FrameworkPropertyMetadata(typeof(CustomDataGrid)));
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
    }
}
