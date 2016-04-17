using System.ComponentModel;
using System.Windows.Controls;

namespace cmdr.Editor.AvalonDock
{
    public class MdiChild<T, U> : INotifyPropertyChanged where T: UserControl
    {
        private static int _mdiChildIndex = 0;

        private T _view;
        public T View
        {
            get { return _view; }
        }

        private U _viewModel;
        public U ViewModel
        {
            get { return _viewModel; }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; raisePropertyChanged("Title"); }
        }


        public MdiChild(T view, U viewModel, string title)
        {
            _view = view;
            _viewModel = viewModel;
            _title = title;

            _id = _mdiChildIndex++.ToString();
            _view.DataContext = _viewModel;
        }


        protected void raisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
