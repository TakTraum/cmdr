using ChangeTracking;
using cmdr.Editor.Utils;
using cmdr.MidiLib;
using cmdr.TsiLib;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ICommand _addMappingCommand;
        public ICommand AddMappingCommand
        {
            get { return _addMappingCommand ?? (_addMappingCommand = new CommandHandler<MenuItemViewModel>(addMapping)); }
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


        private static IList<Mapping> _mappingClipboard;

        public ObservableCollection<MenuItemViewModel> InCommands { get; set; }
        public ObservableCollection<MenuItemViewModel> OutCommands { get; set; }

        public Dictionary<string, AMidiDefinition> DefaultMidiInDefinitions { get; private set; }
        public Dictionary<string, AMidiDefinition> DefaultMidiOutDefinitions { get; private set; }
        

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

            loadDefaultMidiDefinitionsAsync();
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
            var builder = new MenuBuilder<CommandProxy>();
            var itemBuilder = new Func<CommandProxy, MenuItemViewModel>(p => new MenuItemViewModel
                {
                    Text = p.Name,
                    Tag = p
                });

            var allIn = All.KnownInCommands.Select(kv => kv.Value);
            InCommands = new ObservableCollection<MenuItemViewModel>(builder.BuildTree(allIn, itemBuilder, a => a.Category.ToDescriptionString(), "->", false));

            var allOut = All.KnownOutCommands.Select(kv => kv.Value);
            OutCommands = new ObservableCollection<MenuItemViewModel>(builder.BuildTree(allOut, itemBuilder, a => a.Category.ToDescriptionString(), "->", false));
        }

        private void addMapping(MenuItemViewModel item)
        {
            var proxy = item.Tag as CommandProxy;
            var m = _device.CreateMapping(proxy);
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

        private async void loadDefaultMidiDefinitionsAsync()
        {
            if (_device.TypeStr != Device.TYPE_STRING_GENERIC_MIDI)
            {
                var cdm = ControllerDefaultMappings.Instance[_device.TypeStr];
                if (cdm != null)
                {
                    if (cdm.DefaultDevice == null)
                        await cdm.LoadAsync();

                    if (cdm.DefaultDevice != null)
                    {
                        DefaultMidiInDefinitions = cdm.DefaultDevice.MidiInDefinitions.ToDictionary(d => d.Key, d => d.Value);
                        DefaultMidiOutDefinitions = cdm.DefaultDevice.MidiOutDefinitions.ToDictionary(d => d.Key, d => d.Value);
                    }
                }
            }
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
