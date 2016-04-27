using ChangeTracking;
using cmdr.Editor.Utils;
using cmdr.MidiLib;
using cmdr.TsiLib;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class DeviceViewModel : AReversible
    {
        public static readonly string ALL_PORTS = "All Ports";

        private Device _device;

        public int Id { get { return _device.Id; } }

        public string Name { get { return String.IsNullOrWhiteSpace(Comment) ? Type : String.Format("{0} [{1}]", Comment, Type); } }

        public string Type { get { return _device.TypeStr; } }

        public bool IsGenericMidi { get { return Type.Equals(Device.TYPE_STRING_GENERIC_MIDI); } }

        public int Revision { get { return _device.Revision; } }

        public string Comment
        {
            get { return _device.Comment; }
            set { _device.Comment = value; raisePropertyChanged("Comment"); raisePropertyChanged("Name"); IsChanged = true; }
        }

        #region Ports

        public List<string> InPorts { get; private set; }

        public string InPort
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_device.InPort))
                    return ALL_PORTS;
                return _device.InPort;
            }
            set
            {
                if (value == ALL_PORTS)
                    _device.InPort = String.Empty;
                else
                    _device.InPort = value; 
                raisePropertyChanged("InPort"); 
                IsChanged = true;
            }
        }

        public List<string> OutPorts { get; private set; }

        public string OutPort
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_device.OutPort))
                    return ALL_PORTS;
                return _device.OutPort;
            }
            set
            {
                if (value == ALL_PORTS)
                    _device.OutPort = String.Empty;
                else
                    _device.OutPort = value;
                raisePropertyChanged("OutPort");
                IsChanged = true;
            }
        }

        #endregion

        public static IEnumerable<DeviceTarget> Targets { get { return Enum.GetValues(typeof(DeviceTarget)).Cast<DeviceTarget>(); } }

        public DeviceTarget Target
        {
            get { return _device.Target; }
            set { _device.Target = value; raisePropertyChanged("Target"); IsChanged = true; }
        }

        private ObservableCollection<MappingViewModel> _mappings;
        public ObservableCollection<MappingViewModel> Mappings
        {
            get { return _mappings ?? (_mappings = new ObservableCollection<MappingViewModel>()); }
        }

        public IReadOnlyDictionary<string, AMidiDefinition> MidiInDefinitions { get { return _device.MidiInDefinitions; } }
        public IReadOnlyDictionary<string, AMidiDefinition> MidiOutDefinitions { get { return _device.MidiOutDefinitions; } }

        private MappingViewModel _selectedMapping;
        public MappingViewModel SelectedMapping
        {
            get { return _selectedMapping; }
            set { _selectedMapping = value; raisePropertyChanged("SelectedMapping"); }
        }

        private IList<MappingViewModel> _selectedMappings;
        
        private MappingEditorViewModel _mappingEditorViewModel;
        public MappingEditorViewModel MappingEditorViewModel
        {
            get { return _mappingEditorViewModel; }
            set { _mappingEditorViewModel = value; raisePropertyChanged("MappingEditorViewModel"); }
        }

        private SearchViewModel _searchViewModel;
        public SearchViewModel SearchViewModel
        {
            get { return _searchViewModel ?? (_searchViewModel = new SearchViewModel(this)); }
            set { _searchViewModel = value; raisePropertyChanged("SearchViewModel"); }
        }

        #region Commands

        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new CommandHandler(copy, () => _selectedMappings != null && _selectedMappings.Any())); }
        }

        private ICommand _cutCommand;
        public ICommand CutCommand
        {
            get { return _cutCommand ?? (_cutCommand = new CommandHandler(cut, () => _selectedMappings != null && _selectedMappings.Any())); }
        }

        private ICommand _pasteCommand;
        public ICommand PasteCommand
        {
            get { return _pasteCommand ?? (_pasteCommand = new CommandHandler(paste, () => _mappingClipboard != null && _mappingClipboard.Any())); }
        }

        private ICommand _showInCommandsCommand;
        public ICommand ShowInCommandsCommand
        {
            get { return _showInCommandsCommand ?? (_showInCommandsCommand = new CommandHandler<Button>((b) => showCommands(InCommandsMenu, b))); }
        }

        private ICommand _showOutCommandsCommand;
        public ICommand ShowOutCommandsCommand
        {
            get { return _showOutCommandsCommand ?? (_showOutCommandsCommand = new CommandHandler<Button>((b) => showCommands(OutCommandsMenu, b))); }
        }

        private ICommand _removeMappingCommand;
        public ICommand RemoveMappingCommand
        {
            get { return _removeMappingCommand ?? (_removeMappingCommand = new CommandHandler(removeMappings, () => _selectedMappings != null && _selectedMappings.Any())); }
        }

        private ICommand _selectItemsCommand;
        public ICommand SelectItemsCommand
        {
            get { return _selectItemsCommand ?? (_selectItemsCommand = new CommandHandler<IList>(getSelection)); }
        }

        private ICommand _showConditionDescriptionsEditorCommand;
        public ICommand ShowConditionDescriptionsEditorCommand
        {
            get { return _showConditionDescriptionsEditorCommand ?? (_showConditionDescriptionsEditorCommand = new CommandHandler(showConditionDescriptionsEditor)); }
        }

        private ICommand _refreshPortsCommand;
        public ICommand RefreshPortsCommand
        {
            get { return _refreshPortsCommand ?? (_refreshPortsCommand = new CommandHandler(refreshPorts)); }
        }


        #endregion


        public ContextMenu InCommandsMenu { get; set; }
        public ContextMenu OutCommandsMenu { get; set; }

        private static IList<Mapping> _mappingClipboard;

        private static Dictionary<string, List<string>> _controllerDefaultSettings = new Dictionary<string, List<string>>();
        public static IReadOnlyDictionary<string, List<string>> ControllerDefaultSettings
        {
            get { return _controllerDefaultSettings; }
        }



        public DeviceViewModel(Device device)
        {
            _device = device;

            updatePorts(device);

            generateAddMappingContextMenus();

            foreach (var mapping in _device.Mappings)
            {
                var mvm = new MappingViewModel(_device, mapping);
                Mappings.Add(mvm);
                mvm.DirtyStateChanged += (s, e) => updateMapsChanged();
            }

            AcceptChanges();

            Mappings.CollectionChanged += Mappings_CollectionChanged;
        }


        protected override void Accept()
        {
            if (IsChanged)
            {
                _device.IncrementRevision();
                raisePropertyChanged("Revision");
            }
            
            foreach (var mapping in Mappings)
                mapping.AcceptChanges();
        }

        protected override void Revert()
        {
            foreach (var mapping in Mappings)
                mapping.RevertChanges();
        }


        private void getSelection(IList selectedItems)
        {
            _selectedMappings = selectedItems.Cast<MappingViewModel>().ToList();
            MappingEditorViewModel = new MappingEditorViewModel(this, _selectedMappings.ToArray());
        }

        private void updateMapsChanged()
        {
            IsChanged = Mappings.Any(m => m.IsChanged);
        }

        private void showConditionDescriptionsEditor()
        {
            ConditionDescriptions.Generate(Mappings);
            ConditionDescriptions.Edit();
        }

        private void cut()
        {
            copy();
            removeMappings();
        }

        private void copy()
        {
            _mappingClipboard = _selectedMappings.Select(mvm => mvm.Copy(true)).ToList();
        }

        private void paste()
        {
            foreach (var mapping in _mappingClipboard)
            {
                _device.AddMapping(mapping);
                var mvm = new MappingViewModel(_device, mapping);
                _mappings.Add(mvm);
            }
            SelectedMapping = _mappings.Last();
            _mappingClipboard = _mappingClipboard.Select(m => m.Copy(true)).ToList();
        }

        private void generateAddMappingContextMenus()
        {
            var allIn = All.KnownInCommands.Select(kv => kv.Value);
            InCommandsMenu = ContextMenuBuilder<CommandProxy>.Build(allIn, addMapping);

            var allOut = All.KnownOutCommands.Select(kv => kv.Value);
            OutCommandsMenu = ContextMenuBuilder<CommandProxy>.Build(allOut, addMapping);
        }

        private void addMapping(CommandProxy generator)
        {
            var m = _device.CreateMapping(generator);
            _device.AddMapping(m);
            var mvm = new MappingViewModel(_device, m);
            _mappings.Add(mvm);
            SelectedMapping = mvm;
        }

        private void removeMappings()
        {
            foreach (var mvm in _selectedMappings)
            {
                _mappings.Remove(mvm);
                _device.RemoveMapping(mvm.Id);
            }
        }

        private void showCommands(ContextMenu contextMenu, Button button)
        {
            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
            contextMenu.Tag = button.Name;
        }

        private void updatePorts(Device device)
        {
            IEnumerable<string> inPorts = new[] { ALL_PORTS };
            if (!String.IsNullOrWhiteSpace(device.InPort))
                inPorts = inPorts.Union(new[] { device.InPort });
            inPorts = inPorts.Union(MidiManager.InputDevices.Select(d => d.Name));
            InPorts = inPorts.ToList();

            IEnumerable<string> outPorts = new[] { ALL_PORTS };
            if (!String.IsNullOrWhiteSpace(device.OutPort))
                outPorts = outPorts.Union(new[] { device.OutPort });
            outPorts = outPorts.Union(MidiManager.OutputDevices.Select(d => d.Name));
            OutPorts = outPorts.ToList();
        }

        private void refreshPorts()
        {
            MidiManager.RefreshDevices();
            updatePorts(_device);
            raisePropertyChanged("InPorts");
            raisePropertyChanged("InPort");
            raisePropertyChanged("OutPorts");
            raisePropertyChanged("OutPort");
        }

        private static void getControllerDefaultSettings(string rootPath)
        {
            DirectoryInfo di = new DirectoryInfo(rootPath);
            _controllerDefaultSettings = di.EnumerateDirectories().Select(
                d => new
                {
                    Manufacturer = d.Name,
                    ControllerNames = d.EnumerateFiles()
                        .Select(fi =>
                            fi.Name.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                            .Last().Replace(".tsi", ""))
                }).ToDictionary(e => e.Manufacturer, e => e.ControllerNames.ToList());
        }

        void MappingEditorViewModel_DirtyStateChanged(object sender, bool e)
        {
            IsChanged = true;
        }

        #region Events

        void Mappings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (MappingViewModel mvm in e.NewItems)
                        mvm.DirtyStateChanged += (s, a) => updateMapsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (MappingViewModel mvm in e.OldItems)
                        mvm.DirtyStateChanged -= (s, a) => updateMapsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (MappingViewModel mvm in e.OldItems)
                        mvm.DirtyStateChanged -= (s, a) => updateMapsChanged();
                    foreach (MappingViewModel mvm in e.NewItems)
                        mvm.DirtyStateChanged += (s, a) => updateMapsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

            IsChanged = true;
        }

        #endregion

    }
}
