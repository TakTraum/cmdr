using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace cmdr.WpfControls.CustomDataGrid
{
    public class RowItemViewModel : ViewModelBase
    {
        internal DataGrid ParentGrid;

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


        public void BringIntoView()
        {
            if (ParentGrid != null)
                ParentGrid.ScrollIntoView(this);
        }
    }
}
