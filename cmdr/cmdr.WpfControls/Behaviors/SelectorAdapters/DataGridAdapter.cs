using cmdr.WpfControls.Utils;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace cmdr.WpfControls.Behaviors.SelectorAdapters
{
    class DataGridAdapter : ASelectorAdapter
    {
        public DataGridAdapter(Selector selector)
            : base(selector)
        {

        }


        public override bool IsDraggable(DependencyObject control)
        {
            var row = getRow(control);
            // only allow drag for already selected rows
            return (row != null && !row.IsEditing && row.IsSelected);
        }

        public override bool IsValidDropTarget(DependencyObject control)
        {
            if (getRow(control) != null)
                return true;

            // allow drop on empty selector
            var grid = VisualHelpers.FindAncestor<DataGrid>(control);
            if (grid != null && !grid.HasItems)
                return true;

            return false;
        }

        public override bool IsSelected(DependencyObject control)
        {
            var row = getRow(control);
            return (row != null && row.IsSelected);
        }

        public override void Select(DependencyObject control, bool exclusive)
        {
            var row = getRow(control);
            if (row != null)
            {
                if (exclusive && ((Selector as DataGrid).SelectedItems.Count != 1 || !row.IsSelected))
                    Selector.SelectedItem = row.Item;
                else if (!row.IsSelected)
                    row.IsSelected = true;
            }
        }

        public override void Deselect(DependencyObject control)
        {
            var row = getRow(control);
            if (row != null && row.IsSelected)
                row.IsSelected = false;
        }

        public override IList GetSelectedItems()
        {
            return (Selector as DataGrid).SelectedItems;
        }

        public override int GetIndex(DependencyObject control)
        {
            var row = getRow(control);
            if (row != null)
                return row.GetIndex();
            return -1;
        }

        public override FrameworkElement GetItem(DependencyObject control)
        {
            return getRow(control);
        }

        public override void OnDragStarted()
        {
            base.OnDragStarted();
            
            resetSorting();
        }


        private DataGridRow getRow(DependencyObject control)
        {
            return VisualHelpers.FindAncestor<DataGridRow>(control);
        }

        private void resetSorting()
        {
            return;  // pestrela: disabled this because of exception

            var grid = Selector as DataGrid;

            ICollectionView view = CollectionViewSource.GetDefaultView(grid.ItemsSource);
            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();
                foreach (var column in grid.Columns)
                    column.SortDirection = null;
            }
        }
    }
}
