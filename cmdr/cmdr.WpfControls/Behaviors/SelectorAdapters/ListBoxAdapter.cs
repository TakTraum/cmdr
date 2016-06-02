using cmdr.WpfControls.Utils;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace cmdr.WpfControls.Behaviors.SelectorAdapters
{
    class ListBoxAdapter : ASelectorAdapter
    {
        public ListBoxAdapter(Selector selector)
            : base(selector)
        {

        }


        public override bool IsDraggable(DependencyObject control)
        {
            return IsSelected(control);
        }

        public override bool IsValidDropTarget(DependencyObject control)
        {
            if (getListBoxItem(control) != null)
                return true;

            // allow drop on empty selector
            var listbox = VisualHelpers.FindAncestor<ListBox>(control);
            if (listbox != null && !listbox.HasItems)
                return true;

            return false;
        }

        public override FrameworkElement GetItem(DependencyObject control)
        {
            return getListBoxItem(control);
        }

        public override bool IsSelected(DependencyObject control)
        {
            var listBoxItem = getListBoxItem(control);
            return listBoxItem != null && listBoxItem.IsSelected;
        }

        public override void Select(DependencyObject control, bool exclusive)
        {
            var listBoxItem = getListBoxItem(control);
            if (listBoxItem != null)
            {
                if (exclusive)
                    Selector.SelectedItem = listBoxItem.DataContext;
                else
                    listBoxItem.IsSelected = true;
            }
        }

        public override void Deselect(DependencyObject control)
        {
            var listBoxItem = getListBoxItem(control);
            if (listBoxItem != null)
                listBoxItem.IsSelected = false;
        }

        public override IList GetSelectedItems()
        {
            return (Selector as ListBox).SelectedItems;
        }

        public override int GetIndex(DependencyObject control)
        {
            var listBoxItem = getListBoxItem(control);
            if (listBoxItem != null)
                return (Selector as ListBox).Items.IndexOf(listBoxItem.DataContext);
            return -1;
        }




        private ListBoxItem getListBoxItem(DependencyObject control)
        {
            return VisualHelpers.FindAncestor<ListBoxItem>(control);
        }
    }
}
