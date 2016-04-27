using cmdr.Editor.AppSettings;
using cmdr.Editor.AvalonDock;
using cmdr.Editor.Utils;
using cmdr.Editor.Views;
using cmdr.TsiLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;


namespace cmdr.Editor.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        private static readonly string APPNAME = "cmdr";

        private MdiContainer<MdiChild<TsiFileView, TsiFileViewModel>, TsiFileView, TsiFileViewModel> _mdiContainer;
        private ObservableCollection<TsiFileViewModel> _tsiFileViewModels = new ObservableCollection<TsiFileViewModel>();

        private bool _isExiting;

        private string _appTitle = APPNAME;
        public string AppTitle
        {
            get { return _appTitle; }
            set { _appTitle = value; raisePropertyChanged("AppTitle"); }
        }

        #region Commands

        private ICommand _newCommand;
        public ICommand NewCommand
        {
            get { return _newCommand ?? (_newCommand = new CommandHandler(@new)); }
        }

        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new CommandHandler(() => open())); }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new CommandHandler(save, () => SelectedTsiFileModel != null && SelectedTsiFileModel.IsChanged)); }
        }

        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new CommandHandler(saveAs, () => SelectedTsiFileModel != null)); }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new CommandHandler(close, () => SelectedTsiFileModel != null)); }
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new CommandHandler(exit)); }
        }

        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new CommandHandler(copy, canExecuteCopyOrCut)); }
        }

        private ICommand _cutCommand;
        public ICommand CutCommand
        {
            get { return _cutCommand ?? (_cutCommand = new CommandHandler(cut, canExecuteCopyOrCut)); }
        }

        private ICommand _pasteCommand;
        public ICommand PasteCommand
        {
            get { return _pasteCommand ?? (_pasteCommand = new CommandHandler(paste, canExecutePaste)); }
        }

        private ICommand _removeCommand;
        public ICommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = new CommandHandler(remove, canRemove)); }
        }

        private ICommand _learnCommand;
        public ICommand LearnCommand
        {
            get { return _learnCommand ?? (_learnCommand = new CommandHandler(learn, canLearn)); }
        }

        private ICommand _helpCommand;
        public ICommand HelpCommand
        {
            get { return _helpCommand ?? (_helpCommand = new CommandHandler(help)); }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new CommandHandler(about)); }
        }

        private ICommand _dropCommand;
        public ICommand DropCommand
        {
            get { return _dropCommand ?? (_dropCommand = new CommandHandler<IDataObject>(drop)); }
        }

        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get { return _settingsCommand ?? (_settingsCommand = new CommandHandler(showSettings)); }
        }

        #endregion

        private TsiFileViewModel _selectedTsiFileViewModel;
        public TsiFileViewModel SelectedTsiFileModel
        {
            get { return _selectedTsiFileViewModel; }
            set { _selectedTsiFileViewModel = value; raisePropertyChanged("SelectedTsiFileViewModel"); refreshAppTitle(); }
        }


        public ViewModel(DockingManager dockingManager)
        {
            _mdiContainer = new MdiContainer<MdiChild<TsiFileView, TsiFileViewModel>, TsiFileView, TsiFileViewModel>(dockingManager);
            _mdiContainer.OnSelected += onMdiChildSelected;
            _mdiContainer.OnClosing += onMdiChildClosing;
            _mdiContainer.OnClosed += onMdiChildClosed;

            App.Current.MainWindow.Closing += onClosing;

            App.Current.Dispatcher.BeginInvoke((Action)onLoaded);
        }


        #region file methods

        private void @new()
        {
            var fxSettings = (TraktorSettings.Initialized) ? TraktorSettings.Instance.FxSettings : null;
            var vm = new TsiFileViewModel(new TsiFile(CmdrSettings.Instance.TraktorVersion, fxSettings));
            _tsiFileViewModels.Add(vm);
            openTab(vm);
        }

        private bool open()
        {
            string initialDirectory = null;
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.DefaultWorkspace))
                initialDirectory = CmdrSettings.Instance.DefaultWorkspace;
            
            string path = BrowseDialogHelper.BrowseTsiFile(App.Current.MainWindow, false, initialDirectory);
            if (!String.IsNullOrEmpty(path))
            {
                openFile(path);
                return true;
            }

            return false;
        }

        private void save()
        {
            save(SelectedTsiFileModel);
        }

        private void saveAs()
        {
            saveAs(SelectedTsiFileModel);
        }

        private void close()
        {
            _mdiContainer.RemoveMdiChild(_mdiContainer.SelectedMdiChild.Id, true);
        }

        private void exit()
        {
            App.Current.MainWindow.Close();
        }

        #endregion

        #region edit methods

        private void copy()
        {
            SelectedTsiFileModel.SelectedDevice.CopyCommand.Execute(null);
        }

        private void cut()
        {
            SelectedTsiFileModel.SelectedDevice.CutCommand.Execute(null);
        }

        private bool canExecuteCopyOrCut()
        {
            if (SelectedTsiFileModel != null && SelectedTsiFileModel.SelectedDevice != null)
                return SelectedTsiFileModel.SelectedDevice.CopyCommand.CanExecute(null);
            return false;
        }

        private void paste()
        {
            SelectedTsiFileModel.SelectedDevice.PasteCommand.Execute(null);
        }

        private bool canExecutePaste()
        {
            if (SelectedTsiFileModel != null && SelectedTsiFileModel.SelectedDevice != null)
                return SelectedTsiFileModel.SelectedDevice.PasteCommand.CanExecute(null);
            return false;
        }

        private void remove()
        {
            SelectedTsiFileModel.SelectedDevice.RemoveMappingCommand.Execute(null);
        }

        private bool canRemove()
        {
            if (SelectedTsiFileModel != null && SelectedTsiFileModel.SelectedDevice != null)
                return SelectedTsiFileModel.SelectedDevice.RemoveMappingCommand.CanExecute(null);
            return false;
        }

        private void learn()
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.LearnCommand.Execute(null);
        }

        private bool canLearn()
        {
            if (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor != null)
                return SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.LearnCommand.CanExecute(null);
            return false;
        }

        #endregion


        #region help

        private void help()
        {
            System.Diagnostics.Process.Start("https://cmdr.codeplex.com/documentation");
        }

        private void about()
        {
            new AboutWindow(Application.Current.MainWindow).ShowDialog();
        }

        #endregion

        #region extras

        private void showSettings()
        {
            new AppSettingsWindow().ShowDialog();
        }

        #endregion

        private void drop(IDataObject dataObject)
        {
            if (dataObject == null)
                return;

            var fileDrop = dataObject.GetData(DataFormats.FileDrop, true);
            var filesOrDirectories = fileDrop as String[];
            if (filesOrDirectories != null && filesOrDirectories.Length > 0)
            {
                foreach (string fullPath in filesOrDirectories)
                {
                    if (Directory.Exists(fullPath))
                    {
                        IEnumerable<string> files = Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories)
                                        .Where(file => file.ToLower().EndsWith(".tsi"));

                        foreach (var file in files)
                            openFile(file);
                    }
                    else if (File.Exists(fullPath))
                    {
                        if (fullPath.ToLower().EndsWith(".tsi"))
                            openFile(fullPath);
                    }
                }
            }    
        }

        private void openTab(TsiFileViewModel vm)
        {
            var mdiChild = new MdiChild<TsiFileView, TsiFileViewModel>(new TsiFileView(), vm, vm.Title + (vm.IsChanged ? "*" : ""));
            // bind viewmodel's title to MDI child title
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == "Title" || e.PropertyName == "IsChanged") mdiChild.Title = vm.Title + (vm.IsChanged ? "*" : ""); refreshAppTitle(); };
            _mdiContainer.AddMdiChild(mdiChild);
        }

        private void refreshAppTitle()
        {
            if (SelectedTsiFileModel != null)
                AppTitle = String.Format("{0} - {1}", APPNAME, _mdiContainer.SelectedMdiChild.Title);
            else
                AppTitle = APPNAME;
        }

        private bool openFile(string path)
        {
            var mdiChild = _mdiContainer.MdiChildren.Values.FirstOrDefault(c => c.ViewModel.Path == path);
            if (mdiChild == null)
            {
                TsiFile loaded = TsiFile.Load(CmdrSettings.Instance.TraktorVersion, path);
                if (loaded != null)
                {
                    TsiFileViewModel vm = new TsiFileViewModel(loaded);
                    _tsiFileViewModels.Add(vm);
                    openTab(vm);
                }
                else
                {
                    MessageBox.Show("Cannot open file.");
                    return false;
                }
            }
            else
                _mdiContainer.SelectMdiChild(mdiChild.Id);

            return true;
        }

        private void savePendingChanges(out bool cancel)
        {
            cancel = false;
            for (int i = _tsiFileViewModels.Count - 1; i >= 0; i--)
            {
                var vm = _tsiFileViewModels[i];

                if (savePendingChanges(vm, out cancel))
                    if (!save(vm))
                        break;

                if (cancel)
                    break;

                if (_isExiting)
                    close(vm);
            }
        }

        private bool savePendingChanges(TsiFileViewModel vm, out bool cancel)
        {
            cancel = false;
            if (vm.IsChanged && vm.Devices.Any())
            {
                _mdiContainer.SelectMdiChild(_mdiContainer.MdiChildren.Single(m => m.Value.ViewModel.Equals(vm)).Key);
                string msg = "'" + vm.Title + "' was changed. Do you want to save it before exit?";
                var result = MessageBox.Show(msg, "Warning", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                    return true;
                else if (result == MessageBoxResult.Cancel)
                    cancel = true;
            }
            return false;
        }

        private bool save(TsiFileViewModel vm)
        {
            if (vm.Path == null)
                return saveAs(vm);
            else
                return vm.Save(vm.Path);
        }

        private bool saveAs(TsiFileViewModel vm)
        {
            string initialDirectory = null;
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.DefaultWorkspace))
                initialDirectory = CmdrSettings.Instance.DefaultWorkspace;

            string path = BrowseDialogHelper.BrowseTsiFile(App.Current.MainWindow, true, initialDirectory, vm.Title);
            if (!String.IsNullOrEmpty(path))
            {
                vm.Save(path);
                return true;
            }

            return false;
        }

        private void close(TsiFileViewModel vm)
        {
            _mdiContainer.RemoveMdiChild(_mdiContainer.MdiChildren.Single(m => m.Value.ViewModel.Equals(vm)).Key, true);
        }

        #region Events

        void onLoaded()
        {
            // assert app settings are initialized
            while (!CmdrSettings.Instance.Initialized)
            {
                MessageBoxHelper.ShowInfo(
                    "Welcome to cmdr! Before you can map the sh** out of your controllers, please make a few settings." +
                    "\n\nSet at least the targeted Traktor version for your mappings. You can take the default one, even if this means that you won't be able to load and save useful effect selector commands." +
                    "\n\nIf you want the full functionality, you have to setup the path to your \"Traktor Settings.tsi\"." + "\nDon't forget to save settings when you are done.");
                showSettings();
            }

            // try to initialize TraktorSettings
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.PathToTraktorSettings) && !TraktorSettings.Initialized)
            {
                var success = TraktorSettings.Initialize(CmdrSettings.Instance.PathToTraktorSettings, true);
                if (success)
                    return;
            }

            if (!TraktorSettings.Initialized)
                MessageBoxHelper.ShowWarning("Could not load \"Traktor Settings.tsi\"." +
                    "\n\nYou can use cmdr anyway, but in order to load and save useful effect selector commands, you need to setup the path to your \"Traktor Settings.tsi\".");
        }

        void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isExiting = true;
            bool cancel;
            savePendingChanges(out cancel);
            e.Cancel = cancel;
            _isExiting = false;
        }

        void onMdiChildSelected(string id)
        {
            var mdiChild = _mdiContainer.MdiChildren[id];
            SelectedTsiFileModel = mdiChild.ViewModel;
            refreshAppTitle();
        }

        void onMdiChildClosing(string id, CancelEventArgs e)
        {
            if (_isExiting)
                return;

            var mdiChild = _mdiContainer.MdiChildren[id];
            var vm = mdiChild.ViewModel;
            bool cancel;
            if (savePendingChanges(vm, out cancel))
                save(vm);
            e.Cancel = cancel;
        }

        void onMdiChildClosed(string id)
        {
            var mdiChild = _mdiContainer.MdiChildren[id];
            var vm = mdiChild.ViewModel;
            _tsiFileViewModels.Remove(vm);

            if (!_tsiFileViewModels.Any())
                SelectedTsiFileModel = null;
        }

        #endregion
    }
}
