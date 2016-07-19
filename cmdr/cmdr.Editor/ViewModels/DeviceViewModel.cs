using ChangeTracking;
using cmdr.Editor.Metadata;
using cmdr.Editor.Utils;
using cmdr.Editor.ViewModels.Conditions;
using cmdr.MidiLib;
using cmdr.TsiLib;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.WpfControls.Behaviors;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using cmdr.WpfControls.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class DeviceViewModel : AReversible
    {
        public static readonly string ALL_PORTS = "All Ports";
        public static readonly string DEFAULT_PORT = "None";

        private static readonly List<MappingViewModel> _mappingClipboard = new List<MappingViewModel>();
        private static readonly MenuItemViewModel _separator = MenuItemViewModel.Separator;

        private Device _device;
        private string _traktorVersion;

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

        private static Dictionary<MidiEncoderMode, string> _encoderModes;
        public static IReadOnlyDictionary<MidiEncoderMode, string> EncoderModes
        {
            get
            {
                if (_encoderModes == null)
                    _encoderModes = Enum.GetValues(typeof(MidiEncoderMode))
                        .Cast<MidiEncoderMode>()
                        .ToDictionary(d => d, d => d.ToDescriptionString());
                return _encoderModes;
            }
        }
        /// <summary>
        /// Encoder mode, specific to a controller and uniform for all of its encoders. Either 3Fh/41h or 7Fh/01h. Only used for generic midi devices.
        /// </summary>
        public MidiEncoderMode EncoderMode
        {
            get { return _device.EncoderMode; }
            set { _device.EncoderMode = value; raisePropertyChanged("EncoderMode"); IsChanged = true; }
        }

        private ObservableCollection<RowItemViewModel> _mappings = new ObservableCollection<RowItemViewModel>();
        public ObservableCollection<RowItemViewModel> Mappings
        {
            get { return _mappings; }
        }

        public IReadOnlyDictionary<string, AMidiDefinition> MidiInDefinitions { get { return _device.MidiInDefinitions; } }
        public IReadOnlyDictionary<string, AMidiDefinition> MidiOutDefinitions { get { return _device.MidiOutDefinitions; } }

        private ObservableCollection<RowItemViewModel> _selectedMappings = new ObservableCollection<RowItemViewModel>();
        public ObservableCollection<RowItemViewModel> SelectedMappings
        {
            get { return _selectedMappings; }
        }
        
        private MappingEditorViewModel _mappingEditorViewModel;
        public MappingEditorViewModel MappingEditorViewModel
        {
            get { return _mappingEditorViewModel ?? (_mappingEditorViewModel = new MappingEditorViewModel(this, _selectedMappings.Select(r => r.Item as MappingViewModel))); }
            set { _mappingEditorViewModel = value; raisePropertyChanged("MappingEditorViewModel"); }
        }

        private SearchViewModel _searchViewModel;
        public SearchViewModel SearchViewModel
        {
            get { return _searchViewModel ?? (_searchViewModel = new SearchViewModel(this)); }
            set { _searchViewModel = value; raisePropertyChanged("SearchViewModel"); }
        }

        public ObservableCollection<MenuItemViewModel> InCommands { get; private set; }
        public ObservableCollection<MenuItemViewModel> OutCommands { get; private set; }

        public Dictionary<string, AMidiDefinition> DefaultMidiInDefinitions { get; private set; }
        public Dictionary<string, AMidiDefinition> DefaultMidiOutDefinitions { get; private set; }

        private Metadata.Metadata _metadata;
        public Metadata.Metadata Metadata
        {
            get { return _metadata; }
            private set { _metadata = value; raisePropertyChanged("Metadata"); }
        }


        #region Commands

        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new CommandHandler(copy, () => _selectedMappings.Any())); }
        }

        private ICommand _cutCommand;
        public ICommand CutCommand
        {
            get { return _cutCommand ?? (_cutCommand = new CommandHandler(cut, () => _selectedMappings.Any())); }
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
            get
            {
                return _removeMappingCommand ??
                    (_removeMappingCommand = new CommandHandler(() => removeMappings(_selectedMappings), _selectedMappings.Any));
            }
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

        private ICommand _dropCommand;
        public ICommand DropCommand
        {
            get { return _dropCommand ?? (_dropCommand = new CommandHandler<IDataObject>(drop)); }
        }

        private ICommand _selectionChangedCommand;
        public ICommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand ?? (_selectionChangedCommand = new CommandHandler<IList>(updateEditor)); }
        }

        #endregion
        

        public DeviceViewModel(Device device)
        {
            _device = device;
            _traktorVersion = _device.TraktorVersion;

            updatePorts(device);

            generateAddMappingContextMenus();

            loadDefaultMidiDefinitionsAsync();

            foreach (var mapping in _device.Mappings)
            {
                var mvm = new MappingViewModel(_device, mapping);
                Mappings.Add(new RowItemViewModel(mvm));
                mvm.DirtyStateChanged += (s, e) => updateMapsChanged();
            }

            loadMetadata();

            AcceptChanges();

            Mappings.CollectionChanged += Mappings_CollectionChanged;

            // set selection if possible
            if (Mappings.Any())
            {
                SelectedMappings.Add(Mappings.First());
                updateAddMappingContextMenus();
            }
        }


        public void SaveMetadata()
        {
            Metadata.DeviceMetadata.MappingMetadata.Clear();
            Metadata.DeviceMetadata.ConditionDescriptions.Clear();
            foreach (var mapping in Mappings.Select(m => m.Item as MappingViewModel))
            {
                if (mapping.Metadata != null)
                    Metadata.DeviceMetadata.MappingMetadata[mapping.Id] = mapping.Metadata;
                
                if (!String.IsNullOrWhiteSpace(mapping.Conditions.Name))
                    Metadata.DeviceMetadata.ConditionDescriptions[mapping.Conditions.ToString()] = mapping.Conditions.Name;
            }

            _device.TraktorVersion = _traktorVersion + "|" + cmdr.Editor.Metadata.JsonParser.ToJson(Metadata);
        }

        public Device Copy(bool includeMappings)
        {
            var copy = _device.Copy(includeMappings);
            copy.Comment = "";
            return copy;
        }

        protected override void Accept()
        {
            if (IsChanged)
            {
                _device.IncrementRevision();
                raisePropertyChanged("Revision");
            }
            
            foreach (var mapping in Mappings)
                (mapping.Item as MappingViewModel).AcceptChanges();
        }

        protected override void Revert()
        {
            foreach (var mapping in Mappings)
                (mapping.Item as MappingViewModel).RevertChanges();
        }


        private void loadMetadata()
        {
            if (_traktorVersion.Contains("|"))
            {
                var parts = _device.TraktorVersion.Split(new[] { '|' }, 2);

                _traktorVersion = parts[0];

                try
                {
                    Metadata = cmdr.Editor.Metadata.JsonParser.FromJson(parts[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (Metadata != null)
            {
                var mappings = Mappings.Select(m => m.Item as MappingViewModel);
                var conditionTuples = mappings.Select(m => m.Conditions);
                string key;
                foreach (var ct in conditionTuples)
                {
                    key = ct.ToString();
                    if (Metadata.DeviceMetadata.ConditionDescriptions.ContainsKey(key))
                        ct.Name = Metadata.DeviceMetadata.ConditionDescriptions[key];
                }

                foreach (KeyValuePair<int, MappingMetadata> mm in Metadata.DeviceMetadata.MappingMetadata)
                {
                    var mapping = mappings.FirstOrDefault(m => m.Id == mm.Key);
                    if (mapping != null)
                        mapping.Metadata = mm.Value;
                }
            }
            else
                Metadata = new Metadata.Metadata();
        }

        private void drop(IDataObject dataObject)
        {
            if (dataObject == null)
                return;

            var data = dataObject.GetData(typeof(DraggableRowsBehavior.Data)) as DraggableRowsBehavior.Data;
            if (data == null)
                return;

            if (data.TargetIndex < 0 && Mappings.Any()) // // don't allow invalid targets, but allow drop on an empty grid 
                return;

            DeviceViewModel srcDevice = data.SenderDataContext as DeviceViewModel;
            if (srcDevice == null)
                return;

            // copy and sort selected items
            List<RowItemViewModel> selected = new List<RowItemViewModel>(
                data.SelectedItems
                .Cast<RowItemViewModel>()
                .OrderBy(s => srcDevice._mappings.IndexOf(s))
                );

            int newIndex = Math.Max(0, data.TargetIndex);

            if (srcDevice != this || !data.IsMove)
            {
                if (data.IsMove)
                    srcDevice.removeMappings(selected);

                SelectedMappings.Clear();

                foreach (var row in selected)
                {
                    var rawMapping = (row.Item as MappingViewModel).Copy(true);
                    insertMapping(newIndex++, rawMapping);
                }
            }
            else
            {
                var movingAction = new Action<int, int>((oi, ni) =>
                    {
                        _device.MoveMapping(oi, ni);
                        _mappings.Move(oi, ni);
                    });
                MovingLogicHelper.Move<RowItemViewModel>(_mappings, selected, newIndex, movingAction);
            }

            if (selected.Any())
                selected.Last().BringIntoView();
        }

        private void updateMapsChanged()
        {
            IsChanged = Mappings.Any(m => (m.Item as MappingViewModel).IsChanged);
        }

        private void showConditionDescriptionsEditor()
        {
            new Views.ConditionDescriptionsEditor
            {
                DataContext = new ConditionTuplesEditorViewModel(Mappings.Select(r => r.Item as MappingViewModel))
            }
            .ShowDialog();
        }

        private void cut()
        {
            copy();
            removeMappings(_selectedMappings);
        }

        private void copy()
        {
            var sorted = _selectedMappings.OrderBy(s => _mappings.IndexOf(s));

            _mappingClipboard.Clear();
            _mappingClipboard.AddRange(sorted.Select(mvm => mvm.Item as MappingViewModel));
        }

        private void paste()
        {
            var sorted = _selectedMappings.OrderBy(s => _mappings.IndexOf(s));
            
            int index = _mappings.Count;
            if (_selectedMappings.Count > 0)
            {
                index = _mappings.IndexOf(sorted.Last()) + 1;
                SelectedMappings.Clear();
            }

            foreach (var mapping in _mappingClipboard)
                insertMapping(index++, mapping.Copy(true));

            _selectedMappings.Last().BringIntoView();
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

        private void updateAddMappingContextMenus()
        {
            if (OutCommands.Contains(_separator))
            {
                OutCommands.Remove(_separator);
                OutCommands.RemoveAt(OutCommands.Count - 1);
            }
            
            MappingViewModel selectedMapping = null;

            if (_selectedMappings.Count != 1 || (selectedMapping = _selectedMappings.Single().Item as MappingViewModel).Command.MappingType != MappingType.In)
                return;

            if (All.KnownOutCommands.ContainsKey(selectedMapping.Command.Id))
            {
                var commandProxy = All.KnownOutCommands[selectedMapping.Command.Id];
                if (!OutCommands.Contains(_separator))
                {
                    OutCommands.Add(_separator);
                    OutCommands.Add(new MenuItemViewModel
                    {
                        Text = commandProxy.Name + " (" + selectedMapping.AssignmentExpression + ")",
                        Tag = commandProxy,
                        Command = new CommandHandler<MenuItemViewModel>(i =>
                        {
                            int index = _mappings.Count;
                            if (_selectedMappings.Count > 0)
                                index = _mappings.IndexOf(_selectedMappings.Last()) + 1;

                            var proxy = i.Tag as CommandProxy;
                            var m = _device.CreateMapping(proxy);
                            _device.InsertMapping(index, m);

                            var mvm = new MappingViewModel(_device, m);
                            mvm.Assignment = selectedMapping.Assignment;

                            if (selectedMapping.CanOverrideFactoryMap)
                                mvm.OverrideFactoryMap = selectedMapping.OverrideFactoryMap;

                            if (selectedMapping.MidiBinding != null && MidiOutDefinitions.ContainsKey(selectedMapping.MidiBinding.Note))
                                mvm.SetBinding(MidiOutDefinitions[selectedMapping.MidiBinding.Note]);

                            if (selectedMapping.Conditions.Condition1 != null)
                                mvm.SetCondition(TsiLib.Conditions.ConditionNumber.One, selectedMapping.Conditions.Condition1);

                            if (selectedMapping.Conditions.Condition2 != null)
                                mvm.SetCondition(TsiLib.Conditions.ConditionNumber.Two, selectedMapping.Conditions.Condition2);

                            var row = new RowItemViewModel(mvm);
                            _mappings.Insert(index, row);

                            selectExclusive(row);
                            row.BringIntoView();
                        })
                    });
                }
            }
        }

        private void insertMapping(int index, Mapping rawMapping)
        {
            _device.InsertMapping(index, rawMapping);
            var mvm = new MappingViewModel(_device, rawMapping);
            var row = new RowItemViewModel(mvm);
            _mappings.Insert(index, row);

            SelectedMappings.Add(row);
        }

        private void addMapping(MenuItemViewModel item)
        {
            int index = _mappings.Count;
            if (_selectedMappings.Count > 0)
                index = _mappings.IndexOf(_selectedMappings.Last()) + 1;

            var proxy = item.Tag as CommandProxy;
            var m = _device.CreateMapping(proxy);
            _device.InsertMapping(index, m);

            var mvm = new MappingViewModel(_device, m);
            var row = new RowItemViewModel(mvm);
            _mappings.Insert(index, row);

            selectExclusive(row);
            row.BringIntoView();
        }

        private void selectExclusive(RowItemViewModel row)
        {
            SelectedMappings.Clear();
            SelectedMappings.Add(row);
        }

        private void removeMappings(IEnumerable<RowItemViewModel> mappings)
        {
            var selected = new List<RowItemViewModel>(mappings);
            foreach (var m in selected)
            {
                _mappings.Remove(m);
                _device.RemoveMapping((m.Item as MappingViewModel).Id);
            }
        }

        private void updatePorts(Device device)
        {
            IEnumerable<string> inPorts = new[] { DEFAULT_PORT, ALL_PORTS };
            if (!String.IsNullOrWhiteSpace(device.InPort) && !inPorts.Contains(device.InPort))
                inPorts = inPorts.Union(new[] { device.InPort });
            inPorts = inPorts.Union(MidiManager.InputDevices.Select(d => d.Name));
            InPorts = inPorts.ToList();

            IEnumerable<string> outPorts = new[] { DEFAULT_PORT, ALL_PORTS };
            if (!String.IsNullOrWhiteSpace(device.OutPort) && !outPorts.Contains(device.OutPort))
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

        private void updateEditor(IList selection)
        {
            // do not call on CollectionChanged of SelectedMappings! 
            // otherwise it's called too often because e.g. "select all" builds collection of selected items incrementally
            // should be called when selection is complete and not changing anymore. 
            // therefore it's better to use an EventTrigger on DataGrid's SelectionChanged

            var selectedMappingViewModels = _selectedMappings.Select(m => m.Item as MappingViewModel);
            MappingEditorViewModel = new MappingEditorViewModel(this, selectedMappingViewModels);

            updateAddMappingContextMenus();
        }


        #region Events

        void Mappings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (RowItemViewModel row in e.NewItems)
                        (row.Item as MappingViewModel).DirtyStateChanged += (s, a) => updateMapsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (RowItemViewModel row in e.OldItems)
                        (row.Item as MappingViewModel).DirtyStateChanged -= (s, a) => updateMapsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (RowItemViewModel row in e.OldItems)
                        (row.Item as MappingViewModel).DirtyStateChanged -= (s, a) => updateMapsChanged();
                    foreach (RowItemViewModel row in e.NewItems)
                        (row.Item as MappingViewModel).DirtyStateChanged += (s, a) => updateMapsChanged();
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
