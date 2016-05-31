using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cmdr.WpfControls.DropDownButton
{
    public class MenuItemViewModel : ViewModelBase, IComparable
    {
        public static MenuItemViewModel Separator = new MenuItemViewModel { _isSeparator = true  };

        private bool _isSeparator;
        public bool IsSeparator
        {
            get { return _isSeparator; }
        } 


        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; raisePropertyChanged("IsEnabled"); }
        }

        private Uri _iconUri;
        public Uri IconUri
        {
            get { return _iconUri; }
            set { _iconUri = value; raisePropertyChanged("IconUri"); }
        }

        private string _text = String.Empty;
        public string Text
        {
            get { return _text; }
            set { _text = value; raisePropertyChanged("Text"); }
        }

        private ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            set { _command = value; raisePropertyChanged("Command"); }
        }

        private object _tag;
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; raisePropertyChanged("Tag"); }
        }

        private List<MenuItemViewModel> _children = new List<MenuItemViewModel>();
        public List<MenuItemViewModel> Children
        {
            get { return _children; }
            set { _children = value; raisePropertyChanged("Children"); }
        }

        private bool _isCheckable;
        public bool IsCheckable
        {
            get { return _isCheckable; }
            set { _isCheckable = value; raisePropertyChanged("IsCheckable"); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; raisePropertyChanged("IsChecked"); }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1; 
            
            var other = obj as MenuItemViewModel;
            if (other == null)
                throw new ArgumentException();

            if (other.Children.Any() && !Children.Any())
                return 1;
            else if (!other.Children.Any() && Children.Any())
                return -1;

            // simple byte comparison, independent of language
            return StringComparer.OrdinalIgnoreCase.Compare(Text, other.Text);
        }
    }
}
