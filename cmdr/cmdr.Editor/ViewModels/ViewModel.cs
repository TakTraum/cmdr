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

        private string _statusText = "Ready";
        public string StatusText
        {
            get { return _statusText; }
            set { if (value == null) value = "Ready"; _statusText = value; raisePropertyChanged("StatusText"); }
        }

        private MruTsiFiles _mru;
        public MruTsiFiles MRU
        {
            get { return _mru ?? (_mru = new MruTsiFiles(openRecentFile)); }
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
            get { return _saveCommand ?? (_saveCommand = new CommandHandler(save, () => SelectedTsiFileModel != null)); }
        }

        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new CommandHandler(saveAs, () => SelectedTsiFileModel != null)); }
        }

        private ICommand _saveAsCsvCommand;
        public ICommand SaveAsCsvCommand
        {
            get { return _saveAsCsvCommand ?? (_saveAsCsvCommand = new CommandHandler(saveAsCsv, () => SelectedTsiFileModel != null)); }
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

        private ICommand _duplicateCommand;
        public ICommand DuplicateCommand
        {
            get { return _duplicateCommand ?? (_duplicateCommand = new CommandHandler(duplicate, canExecuteCopyOrCut)); }
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

        // pestrela: Increment Note 
        private ICommand _incOneCommand;
        public ICommand IncOneCommand
        {
            get { return _incOneCommand ?? (_incOneCommand = new CommandHandler(() => incDec(1), () => canIncDec(1))); }
        }

        private ICommand _decOneCommand;
        public ICommand DecOneCommand
        {
            get { return _decOneCommand ?? (_decOneCommand = new CommandHandler(() => incDec(-1), () => canIncDec(-1))); }
        }

        // pestrela: Increment Pad feature
        private ICommand _incPadCommand;
        public ICommand IncPadCommand
        {
            get { return _incPadCommand ?? (_incPadCommand = new CommandHandler(() => incDec(8), () => canIncDec(8))); }
        }

        private ICommand _decPadCommand;
        public ICommand DecPadCommand
        {
            get { return _decPadCommand ?? (_decPadCommand = new CommandHandler(() => incDec(-8), () => canIncDec(-8))); }
        }

        // pestrela: Increment Channel feature
        private ICommand _incChCommand;
        public ICommand IncChCommand
        {
            get { return _incChCommand ?? (_incChCommand = new CommandHandler(() => incDecCh(1), () => canIncDecCh(1))); }
        }

        private ICommand _decChCommand;
        public ICommand DecChCommand
        {
            get { return _decChCommand ?? (_decChCommand = new CommandHandler(() => incDecCh(-1), () => canIncDecCh(-1))); }
        }

        /////////
        private ICommand _removeFiltering;
        public ICommand RemoveFiltering
        {
            get { return _removeFiltering ?? (_removeFiltering = new CommandHandler(() => removeFiltering(), () => canRemoveFiltering())); }
        }


        private ICommand _debugDoAction;
        public ICommand DebugDoAction
        {
            get { return _debugDoAction ?? (_debugDoAction = new CommandHandler(() => debugDoAction(), null )); } 
        }

        private ICommand _decAssignment;
        public ICommand DecAssignment
        {
            get {
                    return _decAssignment ?? (
                        _decAssignment= new CommandHandler(() => rotateAssignment(-1), () => canRotateAssignment(-1) ));
            }
        }

        private ICommand _incAssignment;
        public ICommand IncAssignment
        {
            get {
                return _incAssignment?? (
                    _incAssignment= new CommandHandler(() => rotateAssignment(1), () => canRotateAssignment(1)));
            }
        }

        private ICommand _incModifierCommand;
        public ICommand IncModifierCommand
        {
            get
            {
                return _incModifierCommand ?? (
                    _incModifierCommand = new CommandHandler(() => rotateModifierCommand(1), () => canRotateModifierCommand(1)));
            }
        }

        private ICommand _decModifierCommand;
        public ICommand DecModifierCommand
        {
            get
            {
                return _decModifierCommand ?? (
                    _decModifierCommand = new CommandHandler(() => rotateModifierCommand(-1), () => canRotateModifierCommand(-1)));
            }
        }


        private ICommand _bringIntoViewTop;
        public ICommand BringIntoViewTop
        {
            get { return _bringIntoViewTop ?? (_bringIntoViewTop = new CommandHandler(bringIntoViewTop, () => canBringIntoViewTop())); }
        }

        private ICommand _bringIntoViewBottom;
        public ICommand BringIntoViewBottom
        {
            get { return _bringIntoViewBottom ?? (_bringIntoViewBottom = new CommandHandler(bringIntoViewBottom, () => canBringIntoViewBottom())); }
        }


        private ICommand _incModifierValue;
        public ICommand IncModifierValue
        {
            get
            {
                return _incModifierValue ?? (
                    _incModifierValue = new CommandHandler(() => rotateModifierValue(1), () => canRotateModifierValue(1)));
            }
        }

        private ICommand _decModifierValue;
        public ICommand DecModifierValue
        {
            get
            {
                return _decModifierValue ?? (
                    _decModifierValue = new CommandHandler(() => rotateModifierValue(-1), () => canRotateModifierValue(-1)));
            }
        }

        private ICommand _incModifierCondition1;
        public ICommand IncModifierCondition1
        {
            get
            {
                return _incModifierCondition1 ?? (
                    _incModifierCondition1 = new CommandHandler(() => rotateModifierCondition(1, 1), () => canRotateModifierCondition(1, 1)));
            }
        }

        private ICommand _decModifierCondition1;
        public ICommand DecModifierCondition1
        {
            get
            {
                return _decModifierCondition1 ?? (
                    _decModifierCondition1 = new CommandHandler(() => rotateModifierCondition(1, -1), () => canRotateModifierCondition(1, -1)));
            }
        }

        private ICommand _incModifierConditionValue1;
        public ICommand IncModifierConditionValue1
        {
            get
            {
                return _incModifierConditionValue1 ?? (
                    _incModifierConditionValue1 = new CommandHandler(() => rotateModifierConditionValue(1, 1), () => canRotateModifierConditionValue(1, 1)));
            }
        }

        private ICommand _decModifierConditionValue1;
        public ICommand DecModifierConditionValue1
        {
            get
            {
                return _decModifierConditionValue1 ?? (
                    _decModifierConditionValue1 = new CommandHandler(() => rotateModifierConditionValue(1, -1), () => canRotateModifierConditionValue(1, -1)));
            }
        }




        private ICommand _incModifierCondition2;
        public ICommand IncModifierCondition2
        {
            get
            {
                return _incModifierCondition2 ?? (
                    _incModifierCondition2 = new CommandHandler(() => rotateModifierCondition(2, 1), () => canRotateModifierCondition(2, 1)));
            }
        }

        private ICommand _decModifierCondition2;
        public ICommand DecModifierCondition2
        {
            get
            {
                return _decModifierCondition2 ?? (
                    _decModifierCondition2 = new CommandHandler(() => rotateModifierCondition(2,-1), () => canRotateModifierCondition(2,-1)));
            }
        }



        private ICommand _incModifierConditionValue2;
        public ICommand IncModifierConditionValue2
        {
            get
            {
                return _incModifierConditionValue2 ?? (
                    _incModifierConditionValue2 = new CommandHandler(() => rotateModifierConditionValue(2, 1), () => canRotateModifierConditionValue(2, 1)));
            }
        }

        private ICommand _decModifierConditionValue2;
        public ICommand DecModifierConditionValue2
        {
            get
            {
                return _decModifierConditionValue2 ?? (
                    _decModifierConditionValue2 = new CommandHandler(() => rotateModifierConditionValue(2, -1), () => canRotateModifierConditionValue(2, -1)));
            }
        }


        private ICommand _swapConditions;
        public ICommand SwapConditions
        {
            get { return _swapConditions ?? (_swapConditions = new CommandHandler(() => swapConditions(), () => canSwapConditions())); }
        }



        private ICommand _selectAllToggle;
        public ICommand SelectAllToggle
        {
            get { return _selectAllToggle ?? (_selectAllToggle = new CommandHandler(() => selectAllToggle(), () => canSelectAllToggle())); }
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

        private ICommand _showConditionsEditorCommand;
        public ICommand ShowConditionsEditorCommand
        {
            get { return _showConditionsEditorCommand ?? (_showConditionsEditorCommand = new CommandHandler(showConditionsEditor,() => SelectedTsiFileModel != null && SelectedTsiFileModel.SelectedDevice != null)); }
        }

        private ICommand _showCommandsReportEditorCommand;
        public ICommand ShowCommandsReportEditorCommand
        {
            get { return _showCommandsReportEditorCommand ?? (_showCommandsReportEditorCommand = new CommandHandler(showCommandsReportEditor, () => SelectedTsiFileModel != null && SelectedTsiFileModel.SelectedDevice != null)); }
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
            var vm = TsiFileViewModel.Create();
            _tsiFileViewModels.Add(vm);
            openTab(vm);
        }


        private void open_last_mru_file()
        {
            if (!CmdrSettings.Instance.LoadLastFileAtStartup)
            {
                return;
            }

            string path = _mru.get_last_file();
            if (!String.IsNullOrEmpty(path))
            {
                open_path(path);
            }
        }

        private async void open_path(string path)
        {
            if (!String.IsNullOrEmpty(path))
                await openFile(path);
        }

        private async void open()
        {
            string initialDirectory = null;
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.DefaultWorkspace))
                initialDirectory = CmdrSettings.Instance.DefaultWorkspace;

            string path = BrowseDialogHelper.BrowseTsiFile(App.Current.MainWindow, false, initialDirectory);
            open_path(path);
        }

        private async void save()
        {
            await save(SelectedTsiFileModel);
        }

        private async void saveAs()
        {
            await saveAs(SelectedTsiFileModel);
        }

        private async void saveAsCsv()
        {
            await saveAs(SelectedTsiFileModel, "csv");
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


        private void duplicate()
        {
            SelectedTsiFileModel.SelectedDevice.DuplicateCommand.Execute(null);
        }



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

        private void bringIntoViewTop()
        {
            SelectedTsiFileModel.SelectedDevice.BringIntoView(0);
        }

        private bool canBringIntoViewTop()
        {
            return is_mvm_loaded();
        }

        private void bringIntoViewBottom()
        {
            SelectedTsiFileModel.SelectedDevice.BringIntoView(1);
        }

        private bool canBringIntoViewBottom()
        {
            return is_mvm_loaded();
        }

        private void incDec(int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.IncDec(step);
        }

        private bool canIncDec(int step)
        {
            if (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor != null)
                return SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.CanIncDec(step);
            return false;
        }

        private void incDecCh(int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.IncDecCh(step);
        }

        private bool canIncDecCh(int step)
        {
            if (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor != null)
                return SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor.CanIncDecCh(step);
            return false;
        }



        // The grid always has CTRL+a. This is when the focus is elsewhere
        private void selectAllToggle()
        {
            SelectedTsiFileModel.SelectedDevice.selectAllToggle();
        }

        private bool canSelectAllToggle()
        {
            return is_mvm_loaded();
        }


        // this is a dummy action triggered by a shortcut
        // its only here to inspect the pointers of *this in a debugger.
        private void debugDoAction()
        {
            

        }

        private void removeFiltering()
        {
            if(canRemoveFiltering())
            {
                var dev = SelectedTsiFileModel.SelectedDevice;
                dev.Mappings.First().ClearFiltering();
            }
        }

        private bool canRemoveFiltering()
        {
            if(is_dev_loaded())
            {
                var tsi = SelectedTsiFileModel.SelectedDevice;
                if (tsi.Mappings.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool canRotateAssignment(int step)
        {
            return is_mvm_loaded();
        }


        private void rotateAssignment(int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.rotateAssignment(step);
        }


        private bool canRotateModifierCommand(int step)
        {
            return is_mvm_loaded();
        }


        private void rotateModifierCommand(int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.rotateModifierCommand(step);
        }


        private bool canRotateModifierValue(int step)
        {
            return is_mvm_loaded();
        }


        private void rotateModifierValue(int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.rotateModifierValue(step);
        }


        private bool canSwapConditions()
        {
            return is_mvm_loaded();
        }


        private void swapConditions()
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.swapConditions();
        }


        private bool canRotateModifierCondition(int which, int step)
        {
            return is_mvm_loaded();
        }


        private void rotateModifierCondition(int which, int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.rotateModifierCondition(which, step);
        }


        private bool canRotateModifierConditionValue(int which, int step)
        {
            return is_mvm_loaded();
        }

        
        private void rotateModifierConditionValue(int which, int step)
        {
            SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.rotateModifierConditionValue(which, step);
        }


        private bool is_dev_loaded()
        {
            return (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                );
        }

        private bool is_mvm_loaded()
        {
            return (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel != null
                );
        }

        private bool is_mbe_loaded()
        {
            return (SelectedTsiFileModel != null
                && SelectedTsiFileModel.SelectedDevice != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel != null
                && SelectedTsiFileModel.SelectedDevice.MappingEditorViewModel.MidiBindingEditor != null);
        }


        #endregion


        #region help

        private void help()
        {
            System.Diagnostics.Process.Start("https://github.com/pestrela/cmdr/tree/master/docs");
        }

        private void about()
        {
            new AboutWindow(Application.Current.MainWindow).ShowDialog();
        }

        #endregion

        #region extras

        private void showConditionsEditor()
        {
            SelectedTsiFileModel.SelectedDevice.ShowConditionDescriptionsEditorCommand.Execute(null);
        }

        private void showCommandsReportEditor()
        {
            SelectedTsiFileModel.ShowCommandsReportEditorCommand.Execute(null);
        }

        private void showSettings()
        {
            var asw = new AppSettingsWindow();
            asw.Owner = App.Current.MainWindow;
            asw.ShowDialog();
        }

        #endregion

        private async void drop(IDataObject dataObject)
        {
            if (dataObject == null)
                return;

            var fileDrop = dataObject.GetData(DataFormats.FileDrop, true);
            var filesOrDirectories = fileDrop as String[];
            if (filesOrDirectories != null && filesOrDirectories.Length > 0)
            {
                List<Task> tasks = new List<Task>();
                
                // collect tasks first
                foreach (string fullPath in filesOrDirectories)
                {
                    if (Directory.Exists(fullPath))
                    {
                        IEnumerable<string> files = Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories)
                                        .Where(file => file.ToLower().EndsWith(".tsi"));
                        tasks.AddRange(files.Select(f => openFile(f)));
                    }
                    else if (File.Exists(fullPath) && fullPath.ToLower().EndsWith(".tsi"))
                        tasks.Add(openFile(fullPath));
                }
                
                // now execute
                await Task.WhenAll(tasks);
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
            if (SelectedTsiFileModel != null && _mdiContainer.SelectedMdiChild != null)
                AppTitle = String.Format("{0} - {1}", APPNAME, _mdiContainer.SelectedMdiChild.Title);
            else
                AppTitle = APPNAME;

            // pestrela: avoid confusion by removing grid filtering always
            // it would be better to reaply filtering in the new device list
            //
            // note2: check later if we can call this less often. For example not in "add command"
            removeFiltering();

        }

        private async Task openFile(string path)
        {
            var mdiChild = _mdiContainer.MdiChildren.Values.FirstOrDefault(c => c.ViewModel.Path == path);
            if (mdiChild == null)
            {
                TsiFileViewModel vm = await TsiFileViewModel.LoadAsync(path);
                if (vm != null)
                {
                    _tsiFileViewModels.Add(vm);
                    openTab(vm);
                    _mru.Add(path);
                }
                else
                    MessageBoxHelper.ShowError("Cannot open file.\nPlease upload the TSI in https://github.com/pestrela/cmdr/issues");
            }
            else
                _mdiContainer.SelectMdiChild(mdiChild.Id);
        }

        private async Task<bool> savePendingChanges()
        {
            bool cancel = false;
            for (int i = _tsiFileViewModels.Count - 1; i >= 0; i--)
            {
                var vm = _tsiFileViewModels[i];

                if (savePendingChanges(vm, out cancel))
                    if (! await save(vm))
                        break;

                if (cancel)
                    break;

                if (_isExiting)
                    close(vm);
            }

            return !cancel;
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

        private async Task<bool> save(TsiFileViewModel vm)
        {
            if (vm.Path == null)
                return await saveAs(vm);
            else
                return await vm.SaveAsyncTsi(vm.Path);
        }

        private async Task<bool> saveAs(TsiFileViewModel vm, string type = "tsi")
        {
            string initialDirectory = null;
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.DefaultWorkspace))
                initialDirectory = CmdrSettings.Instance.DefaultWorkspace;

            string path = BrowseDialogHelper.BrowseTsiFile(App.Current.MainWindow, true, initialDirectory, vm.Title, type);
            if (!String.IsNullOrEmpty(path))
            {
                switch(type) {
                    case "tsi":
                        return await vm.SaveAsyncTsi(path);
                    case "csv":
                        return await vm.SaveAsyncCsv(path);
                }
            }

            return false;
        }

        private async Task<bool> saveAsTsi(TsiFileViewModel vm)
        {
            return await saveAs(vm, "tsi");
        }

        private async Task<bool> saveAsCsv(TsiFileViewModel vm)
        {
            return await saveAs(vm, "csv");
        }

        private void close(TsiFileViewModel vm)
        {
            _mdiContainer.RemoveMdiChild(_mdiContainer.MdiChildren.Single(m => m.Value.ViewModel.Equals(vm)).Key, true);
        }

        private async void initialize()
        {
            // assert app settings are initialized
            while (!CmdrSettings.Instance.Initialized)
            {
                App.SetStatus("Initializing App Settings ...");
                MessageBoxHelper.ShowInfo(
                    "UPDATE JAN 2020:\n" +
                    "****************\n "+
                    " Please see the new features in the changelog: https://github.com/pestrela/cmdr/blob/master/docs/development/Change_Log.md\n" +
                    "\n\n" +
                    "Original Message:\n"+
                    "******************\n" +
                    "Before you can map your controllers, please make a few settings. " +
                    "Set at least the targeted Traktor version for your mappings. You can take the default one." +
                    "\n\nIf you want the full functionality, please setup the paths to your \"Traktor Settings.tsi\" and " + 
                    "your Controller Default Mappings." +
                    "\n\nDon't forget to save settings when you are done.", "Welcome to cmdr!");
                showSettings();
                App.ResetStatus();
            }

            // try to initialize TraktorSettings
            if (!String.IsNullOrEmpty(CmdrSettings.Instance.PathToTraktorSettings) && !TraktorSettings.Initialized)
                await Task.Factory.StartNew(() =>
                {
                    App.SetStatus("Initializing Traktor Settings ..."); 
                    TraktorSettings.Initialize(CmdrSettings.Instance.PathToTraktorSettings, true);
                    App.ResetStatus();
                });

            _mru.Load();

            open_last_mru_file();

            await ControllerDefaultMappings.Instance.LoadAsync(CmdrSettings.Instance.PathToControllerDefaultMappings);
        }

        private void openRecentFile(string file)
        {
            if (!File.Exists(file))
            {
                var yes = MessageBoxHelper.ShowQuestion("Cannot open '" + file + "'.\nDo you want to remove it from the list of recently opened files?");
                if (yes)
                    _mru.Remove(file);
            }
            else
                openFile(file);
        }

        #region Events

        void onLoaded()
        {
            initialize();
        }

        private async void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isExiting = true;
            bool cancel = !await savePendingChanges();
            e.Cancel = cancel;
            _isExiting = false;
        }

        void onMdiChildSelected(string id)
        {
            var mdiChild = _mdiContainer.MdiChildren[id];
            SelectedTsiFileModel = mdiChild.ViewModel;
            refreshAppTitle();
        }

        private async void onMdiChildClosing(string id, CancelEventArgs e)
        {
            if (_isExiting)
                return;

            var mdiChild = _mdiContainer.MdiChildren[id];
            var vm = mdiChild.ViewModel;
            bool cancel;
            if (savePendingChanges(vm, out cancel))
                await save(vm);
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
