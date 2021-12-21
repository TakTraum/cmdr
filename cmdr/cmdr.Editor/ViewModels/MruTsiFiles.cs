using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.WpfControls.DropDownButton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cmdr.Editor.ViewModels
{
    public class MruTsiFiles : ViewModelBase
    {
        private static readonly int CAPACITY = 10;

        private MruSection _configSection;
        private List<string> _files;
        private Action<string> _fileOpenCallback;

        private bool _hasMenuItems;
        public bool HasMenuItems
        {
            get { return _hasMenuItems; }
            set { _hasMenuItems = value; raisePropertyChanged("HasMenuItems"); }
        }

        private ObservableCollection<MenuItemViewModel> _menuItems;
        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get { return _menuItems; }
            set { _menuItems = value; raisePropertyChanged("MenuItems"); }
        }


        public MruTsiFiles(Action<string> fileOpenCallback)
        {
            _fileOpenCallback = fileOpenCallback;
            _files = new List<string>();
            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }


        public void Add(string file)
        {
            Remove(file);

            _files.Insert(0, file);

            if (_files.Count > CAPACITY)
                _files.RemoveAt(_files.Count - 1);

            save();
        }

        public void Remove(string file)
        {
            if (!_files.Contains(file))
                return;

            _files.Remove(file);
            save();
        }

        public void Load()
        {
            if (_configSection == null)
                _configSection = AppSettings.CmdrSettings.Instance.MRU;

            _files.Clear();
            foreach (var element in _configSection.Elements)
                _files.Add(element.Path);

            generateMenuItems();
        }


        public string get_last_file()
        {
            var ret = (_files.Count() > 0 ? _files[0] : null);
            return ret;
        }

        private void save()
        {
            if (_configSection == null)
                return;

            _configSection.Elements.Clear();

            foreach (var file in _files)
                _configSection.Elements.Add(new MruElement { Path = file });

            CmdrSettings.Instance.Save();

            generateMenuItems();
        }

        private void generateMenuItems()
        {
            var items = _files.Select(f => new MenuItemViewModel
            {
                Text = f,
                Command = (_fileOpenCallback != null) ? new CommandHandler(() => _fileOpenCallback(f)) : null
            });

            MenuItems = new ObservableCollection<MenuItemViewModel>(items);
            HasMenuItems = MenuItems.Any();
        }
    }
}
