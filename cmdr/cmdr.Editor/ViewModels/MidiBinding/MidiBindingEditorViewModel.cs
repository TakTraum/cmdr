using cmdr.Editor.Utils;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.WpfControls.DropDownButton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Threading;

namespace cmdr.Editor.ViewModels.MidiBinding
{
    public class MidiBindingEditorViewModel : ViewModelBase
    {
        private static readonly MenuItemViewModel SEPARATOR = MenuItemViewModel.Separator;

        private readonly DeviceViewModel _device;
        private readonly IEnumerable<MappingViewModel> _mappings;
        private readonly IEnumerable<AMidiDefinition> _proprietaryDefinitions;
        private readonly MidiLearner _midiLearner;


        public bool IsGenericMidi { get { return _device.IsGenericMidi; } }

        private readonly bool _canOverrideFactoryMap;
        public bool CanOverrideFactoryMap { get { return _canOverrideFactoryMap; } }

        public bool? OverrideFactoryMap
        {
            get
            {
                var common = _mappings.Select(m => m.OverrideFactoryMap).Distinct();
                if (common.Count() == 1)
                    return common.Single();
                else
                    return null;
            }
            set
            {
                foreach (var mvm in _mappings)
                    mvm.OverrideFactoryMap = value.HasValue ? value.Value : false;
                raisePropertyChanged("OverrideFactoryMap");
            }
        }

        private bool _comboMode;
        public bool ComboMode
        {
            get { return _comboMode; }
            set { _comboMode = value; raisePropertyChanged("ComboMode"); }
        }

        private bool _learnMode;
        public bool LearnMode
        {
            get { return _learnMode; }
            set { _learnMode = value; raisePropertyChanged("LearnMode"); }
        }

        #region Channel

        private List<string> _selectedChannels;
        private MenuItemViewModel _selectedChannelsMenuItem = new MenuItemViewModel { Text = "Selected Channels" };

        private string _channel;
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; updateBinding(); raisePropertyChanged("ChannelString"); }
        }

        public string ChannelString
        {
            get { return VariousChannels ? "Various" : String.IsNullOrEmpty(Channel) ? "None" : Channel; }
        }

        public ObservableCollection<MenuItemViewModel> ChannelsMenu { get; private set; }

        private bool _variousChannels;
        public bool VariousChannels { get { return _variousChannels; } }

        #endregion

        #region Note

        private List<object> _selectedNotes;
        private MenuItemViewModel _selectedNotesMenuItem = new MenuItemViewModel { Text = "Selected Notes" };

        private string _note;
        public string Note
        {
            get { return _note; }
            set
            {
                if (ComboMode && !String.IsNullOrEmpty(value))
                {
                    _note += "+" + value;
                    ComboMode = false;
                }
                else
                    _note = value;

                if (IsGenericMidi)
                    updateBinding();

                raisePropertyChanged("NoteString");
            }
        }

        public string NoteString
        {
            get { return VariousNotes ? "Various" : String.IsNullOrEmpty(Note) ? "None" : Note; }
        }

        public ObservableCollection<MenuItemViewModel> NotesMenu { get; private set; }

        private bool _variousNotes;
        public bool VariousNotes
        {
            get { return _variousNotes; }
        }

        #endregion

        #region Commands

        private ICommand _selectNoteCommand;
        public ICommand SelectNoteCommand
        {
            get { return _selectNoteCommand ?? (_selectNoteCommand = new CommandHandler<MenuItemViewModel>(selectNote)); }
        }

        private ICommand _selectChannelCommand;
        public ICommand SelectChannelCommand
        {
            get { return _selectChannelCommand ?? (_selectChannelCommand = new CommandHandler<MenuItemViewModel>(selectChannel)); }
        }

        private ICommand _learnCommand;
        public ICommand LearnCommand
        {
            get { return _learnCommand ?? (_learnCommand = new CommandHandler(toggleLearn, canLearn));}
        }

        private ICommand _resetCommand;
        public ICommand ResetCommand
        {
            get { return _resetCommand ?? (_resetCommand = new CommandHandler(reset)); }
        }

        private ICommand _comboCommand;
        public ICommand ComboCommand
        {
            get { return _comboCommand ?? (_comboCommand = new CommandHandler(toggleCombo, canCombo)); }
        }

        #endregion


        private MidiBindingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappings, IEnumerable<AMidiDefinition> proprietaryDefinitions = null)
        {
            _mappings = mappings;
            _device = device;

            if (IsGenericMidi)
            {
                ChannelsMenu = new ObservableCollection<MenuItemViewModel>(generateChannelsMenu());
                NotesMenu = new ObservableCollection<MenuItemViewModel>(NotesMenuBuilder.Instance.BuildGenericMidiMenu());
                _midiLearner = new MidiLearner((signal) =>
                {
                    toggleLearn();

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        setChannel(String.Format("Ch{0:00}", signal.Channel));
                        Note = signal.Note;

                        // workaround to refresh combo and reset button
                        (ComboCommand as CommandHandler).UpdateCanExecuteState();
                        (ResetCommand as CommandHandler).UpdateCanExecuteState();
                    }));
                });
            }
            else
            {
                _canOverrideFactoryMap = _mappings.Any(m => m.CanOverrideFactoryMap);
                _proprietaryDefinitions = proprietaryDefinitions;
                NotesMenu = new ObservableCollection<MenuItemViewModel>(NotesMenuBuilder.Instance.BuildProprietaryMenu(_proprietaryDefinitions.DistinctBy(d => d.Note)));
            }

            updateCommonChannelAndNote();
        }


        private void setChannel(string channel)
        {
            _channel = channel;
            raisePropertyChanged("ChannelString");
        }

        private void setNote(string note)
        {
            _note = note;
            raisePropertyChanged("NoteString");
        }

        private void updateBinding(AMidiDefinition definition = null)
        {
            string expression;
            AMidiDefinition tmpDefinition;
            foreach (var mapping in _mappings)
            {
                expression = null;
                tmpDefinition = definition;

                if (IsGenericMidi)
                {
                    if (!String.IsNullOrEmpty(_channel))
                    {
                        if (!String.IsNullOrEmpty(_note))
                            expression = _channel + "." + _note.Replace("+", "+" + _channel + ".");
                        else if (mapping.MidiBinding != null)
                            expression = Regex.Replace(mapping.MidiBinding.Note, @"Ch\d+", _channel);
                    }
                    else if (!String.IsNullOrEmpty(_note))
                    {
                        if (mapping.MidiBinding != null)
                        {
                            var channel = mapping.MidiBinding.Note.Substring(0, 4);
                            expression = channel + "." + _note.Replace("+", "+" + channel + ".");
                        }
                    }

                    if (expression != null)
                        tmpDefinition = new GenericMidiDefinition(mapping.Command.MappingType, expression);
                }
                else
                {
                    if (tmpDefinition != null && mapping.Command.MappingType != tmpDefinition.Type)
                        tmpDefinition = _proprietaryDefinitions.First(p => p.Type == mapping.Command.MappingType && p.Note == tmpDefinition.Note);
                }

                mapping.SetBinding(tmpDefinition);
            }

            updateCommonChannelAndNote();
        }

        private void updateCommonChannelAndNote()
        {
            var bindings = _mappings.Select(mvm => mvm.MidiBinding).Distinct();
            var notes = bindings.Select(b => (b != null) ? b.Note : null).Distinct();

            if (IsGenericMidi)
            {
                var parts = notes.Select(n =>
                {
                    if (n == null)
                        return null;
                    var ch = n.Substring(0, 4);
                    return new { Channel = ch, Note = n.Replace(ch + ".", "") };
                });

                var channels = parts.Select(p => (p != null) ? p.Channel : null).Distinct();

                _selectedChannels = channels.Where(c => c != null).OrderBy(c => c).ToList();

                _variousChannels = channels.Count() > 1;
                if (!_variousChannels && String.IsNullOrEmpty(_channel))
                    setChannel(channels.Single());

                updateChannelsMenu(_selectedChannels);

                notes = parts.Select(p => (p != null) ? p.Note : null).Distinct();

                _selectedNotes = notes.Where(n => n != null).OrderBy(n => n).Cast<object>().ToList();
            }
            else
                _selectedNotes = _proprietaryDefinitions.Where(n => notes.Contains(n.Note)).DistinctBy(d => d.Note).OrderBy(d => d.Note).Cast<object>().ToList();

            _variousNotes = notes.Count() > 1;
            if (!_variousNotes && String.IsNullOrEmpty(_note))
                setNote(notes.Single());

            updateNotesMenu(_selectedNotes);
        }

        private void reset()
        {
            if (IsGenericMidi)
                setChannel(null);
            else
                updateBinding();
            Note = String.Empty;
        }

        private void toggleLearn()
        {
            if (LearnMode)
            {
                _midiLearner.Stop();
                LearnMode = false;
            }
            else
            {
                if (_midiLearner.Start())
                    LearnMode = true;
            }
        }

        private bool canLearn()
        {
            if (!IsGenericMidi)
                return false;

            if (_device.InPort.Equals(DeviceViewModel.ALL_PORTS))
                return _midiLearner.CanLearn();
            else
                return _midiLearner.CanLearn(_device.InPort);
        }

        private void toggleCombo()
        {
            if (ComboMode)
                ComboMode = false;
            else
                ComboMode = true;
        }

        private bool canCombo()
        {
            if (!IsGenericMidi)
                return false;

            return !(String.IsNullOrEmpty(Note) || Note.Contains("+") || VariousChannels);
        }


        #region ContextMenus

        #region Channels

        private void selectChannel(MenuItemViewModel item)
        {
            Channel = item.Tag.ToString();
        }

        private List<MenuItemViewModel> generateChannelsMenu()
        {
            return Enumerable.Range(1, 16).Select(c =>
            {
                var str = String.Format("Ch{0:00}", c);
                return new MenuItemViewModel { Text = str, Tag = str };
            }).ToList();
        }

        private void updateChannelsMenu(IEnumerable<string> selectedChannels)
        {
            if (!_selectedChannels.Where(c => c != _channel).Any())
            {
                if (ChannelsMenu.Contains(_selectedChannelsMenuItem))
                {
                    ChannelsMenu.Remove(SEPARATOR);
                    ChannelsMenu.Remove(_selectedChannelsMenuItem);
                }
                
                return;
            }

            _selectedChannelsMenuItem.Children = _selectedChannels.Select(c => new MenuItemViewModel { Text = c, Tag = c }).ToList();

            if (!ChannelsMenu.Contains(_selectedChannelsMenuItem))
            {
                ChannelsMenu.Add(SEPARATOR);
                ChannelsMenu.Add(_selectedChannelsMenuItem);
            }
        }

        #endregion

        #region Notes

        private void selectNote(MenuItemViewModel item)
        {
            var definition = item.Tag as AMidiDefinition;
            if (definition != null) // proprietary
            {
                // it's ok to set a reference here, as there should be a single definition per note anyway
                updateBinding(definition);
                Note = definition.Note;
            }
            else // generic midi
                Note = item.Tag.ToString();
        }

        private void updateNotesMenu(IEnumerable<object> selectedNotes)
        {
            var hasVariousNotesUnequalNull = (IsGenericMidi && !_selectedNotes.Where(n => n.ToString() != _note).Any()) ||
                (!IsGenericMidi && !_selectedNotes.Cast<AMidiDefinition>().Where(n => n.Note != _note).Any());

            if (hasVariousNotesUnequalNull)
            {
                if (NotesMenu.Contains(_selectedNotesMenuItem))
                {
                    NotesMenu.Remove(SEPARATOR);
                    NotesMenu.Remove(_selectedNotesMenuItem);
                }
                return;
            }

            _selectedNotesMenuItem.Children = NotesMenuBuilder.Instance.BuildSelectedNotesMenu(selectedNotes);

            if (!NotesMenu.Contains(_selectedNotesMenuItem))
            {
                NotesMenu.Add(SEPARATOR);
                NotesMenu.Add(_selectedNotesMenuItem);
            }
        }

        #endregion

        #endregion


        public static MidiBindingEditorViewModel BuildEditor(DeviceViewModel device, IEnumerable<MappingViewModel> mappings)
        {
            if (mappings.Any())
            {
                if (!device.IsGenericMidi)
                {
                    var propDefs = getProprietaryDefinitions(device, mappings);
                    if (propDefs.Any())
                        return new MidiBindingEditorViewModel(device, mappings, propDefs);
                }
                else
                    return new MidiBindingEditorViewModel(device, mappings);
            }

            return null; // either no mapping selected or no (valid) definitions available
        }


        private static IEnumerable<AMidiDefinition> getProprietaryDefinitions(DeviceViewModel device, IEnumerable<MappingViewModel> mappings)
        {
            IEnumerable<KeyValuePair<string, AMidiDefinition>> inDefinitions = null, outDefinitions = null;

            // get definitions from current tsi file as well as default definitions (if available)

            bool hasInMappings = mappings.Any(m => m.Command.MappingType == MappingType.In);
            if (hasInMappings)
            {
                inDefinitions = device.MidiInDefinitions;
                if (device.DefaultMidiInDefinitions != null)
                    inDefinitions = inDefinitions.Union(device.DefaultMidiInDefinitions);
            }

            bool hasOutMappings = mappings.Any(m => m.Command.MappingType == MappingType.Out);
            if (hasOutMappings)
            {
                outDefinitions = device.MidiOutDefinitions;
                if (device.DefaultMidiOutDefinitions != null)
                    outDefinitions = outDefinitions.Union(device.DefaultMidiOutDefinitions);
            }

            if (hasInMappings && hasOutMappings) // get intersection of In and Out definitions
                return (from inDef in inDefinitions
                        join outDef in outDefinitions on inDef.Key equals outDef.Key
                        where (!isDefault(outDef))
                        select new[] { inDef.Value, outDef.Value }).SelectMany(d => d);
            else if (hasInMappings)
                return inDefinitions.Select(kv => kv.Value);
            else
                return outDefinitions.Select(kv => kv.Value);
        }

        private static bool isDefault<T>(T value) where T : struct
        {
            return value.Equals(default(T));
        }
    }
}
