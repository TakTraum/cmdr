using cmdr.WpfControls.Utils;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using cmdr.WpfControls.CustomDataGrid;

namespace cmdr.WpfControls.ViewModels
{
    public class RowItemViewModel : ViewModelBase
    {
        //internal Selector ParentSelector;   // HACK to enable clear filtering with zero mappings
        public Selector ParentSelector;

        private object _item;
        public object Item
        {
            get { return _item; }
            set { _item = value; raisePropertyChanged("Item"); }
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { _isHighlighted = value; raisePropertyChanged("IsHighlighted"); }
        }


        public RowItemViewModel()
        {

        }

        public RowItemViewModel(object item)
        {
            _item = item;
        }

        public void check_parent_selector()
        {
            if (ParentSelector == null) {
                throw new InvalidOperationException("Parent Selector is null");
            }
                
            if (!(ParentSelector is CustomDataGrid.CustomDataGrid)) {
                throw new InvalidOperationException("Target object has no ClearFiltering method");
            }
        }

        public void ClearFiltering()
        {
           check_parent_selector();

           (ParentSelector as CustomDataGrid.CustomDataGrid).ClearFiltering();
        }

        public void ReApplyFiltering()
        {
            check_parent_selector();

            (ParentSelector as CustomDataGrid.CustomDataGrid).ReApplyFiltering();
        }

        // this was the only method made by TakTraum to access the datagrid
        public void BringIntoView()
        {
            if (ParentSelector == null)
                return;

            if (ParentSelector is DataGrid)
                (ParentSelector as DataGrid).ScrollIntoView(this);
            else if (ParentSelector is ListBox)
                (ParentSelector as ListBox).ScrollIntoView(this);
            else
                throw new InvalidOperationException("Target object has no ScrollIntoView method.");
        }
    }
}
