using cmdr.WpfControls.CustomDataGrid;
using cmdr.WpfControls.Utils;
using cmdr.WpfControls.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.WpfControls.Test
{
    public class ViewModel : ViewModelBase
    {
        public class Thing : ViewModelBase
        {
            private int _number;
            public int Number
            {
                get { return _number; }
                set { _number = value; raisePropertyChanged("Number"); }
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public Thing Copy()
            {
                return this.MemberwiseClone() as Thing;
            }
        }

        private ObservableCollection<RowItemViewModel> _things = new ObservableCollection<RowItemViewModel>();
        public ObservableCollection<RowItemViewModel> Things
        {
            get { return _things; }
            set { _things = value; raisePropertyChanged("Things"); }
        }

        private ObservableCollection<RowItemViewModel> _selectedThings = new ObservableCollection<RowItemViewModel>();
        public ObservableCollection<RowItemViewModel> SelectedThings
        {
            get { return _selectedThings; }
            set { _selectedThings = value; raisePropertyChanged("SelectedThings"); check(); }
        }

        private ObservableCollection<RowItemViewModel> _highlightedThings = new ObservableCollection<RowItemViewModel>();
        public ObservableCollection<RowItemViewModel> HighlightedThings
        {
            get { return _highlightedThings; }
            set { _highlightedThings = value; raisePropertyChanged("HighlightedThings"); }
        }


        public ViewModel()
        {
            var items = Enumerable.Range(1, 10).Select(i =>
                new RowItemViewModel
                {
                    Item = new Thing
                    {
                        Number = i,
                        Name = "Name " + i,
                    }
                }
            );

            _things = new ObservableCollection<RowItemViewModel>(items);

            SelectedThings.Add(_things[2]);
            SelectedThings.Add(_things[3]);
        }

        private void check()
        {
            if (SelectedThings == null || SelectedThings.Count == 0)
                return;

            foreach (var t in Things)
            {
                var thing = t.Item as Thing;
                t.IsHighlighted = thing.Number % ((SelectedThings[0] as RowItemViewModel).Item as Thing).Number == 0;
            }            
        }
    }
}
