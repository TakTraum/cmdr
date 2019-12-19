using cmdr.WpfControls.Utils;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using cmdr.WpfControls.CustomDataGrid;

namespace cmdr.WpfControls.ViewModels
{
    public class RowItemViewModel : ViewModelBase
    {
        internal Selector ParentSelector;

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

        public void ClearFiltering()
        {
            if (ParentSelector == null)
                return;

            if (ParentSelector is CustomDataGrid.CustomDataGrid)
                (ParentSelector as CustomDataGrid.CustomDataGrid).ClearFiltering();
            else
                throw new InvalidOperationException("Target object has no ClearFiltering method.");
        }

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
