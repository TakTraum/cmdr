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
    public class MenuItemViewModel : ViewModelBase
    {
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; raisePropertyChanged("IsEnabled"); }
        }

        private Uri _menuIcon;
        public Uri MenuIcon
        {
            get { return _menuIcon; }
            set { _menuIcon = value; raisePropertyChanged("MenuIcon"); }
        }

        private string _menuText = String.Empty;
        public string MenuText
        {
            get { return _menuText; }
            set { _menuText = value; raisePropertyChanged("MenuText"); }
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

        private ObservableCollection<MenuItemViewModel> _children = new ObservableCollection<MenuItemViewModel>();
        public ObservableCollection<MenuItemViewModel> Children
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
    }
}
