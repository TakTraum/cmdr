using System.ComponentModel;

namespace cmdr.Editor.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
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
