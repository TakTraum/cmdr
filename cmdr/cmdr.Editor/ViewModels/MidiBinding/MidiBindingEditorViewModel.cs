using cmdr.Editor.Utils;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.TsiLib.MidiDefinitions.GenericMidi;
using cmdr.WpfControls.DropDownButton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

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

        public ObservableCollection<MenuItemViewModel> IncDecMenu { get; private set; }

        private bool _isCCs = false;
        private bool _isNotes = false;
        private bool _hasCombo = false;
        private int _min = 0;
        private int _max = 0;

        private int _min_ch = 1;
        private int _max_ch = 10;
        private bool _allBound = false;

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

        private ICommand _incDecCommand;
        public ICommand IncDecCommand
        {
            get { return _incDecCommand ?? (_incDecCommand = new CommandHandler<MenuItemViewModel>(item => IncDec((int)item.Tag), () => CanIncDec())); }
        }

        private ICommand _applyMidiRangeCommand;
        public ICommand ApplyMidiRangeCommand
        {
            get { return _applyMidiRangeCommand ?? (_applyMidiRangeCommand = new CommandHandler(applyMidiRange, canApplyMidiRange)); }
        }

        #endregion


        private MidiBindingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappings, IEnumerable<AMidiDefinition> proprietaryDefinitions = null)
        {
            _mappings = mappings;
            _device = device;
            _proprietaryDefinitions = proprietaryDefinitions;

            analyzeSelection();

            if (IsGenericMidi)
            {
                ChannelsMenu = new ObservableCollection<MenuItemViewModel>(generateChannelsMenu());
                NotesMenu = new ObservableCollection<MenuItemViewModel>(NotesMenuBuilder.Instance.BuildGenericMidiMenu());
                IncDecMenu = new ObservableCollection<MenuItemViewModel>(generateIncDecMenu());

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
                NotesMenu = new ObservableCollection<MenuItemViewModel>(NotesMenuBuilder.Instance.BuildProprietaryMenu(_proprietaryDefinitions.DistinctBy(d => d.Note)));
            }

            updateMenus();
        }

        public void IncDec(int step)
        {
            foreach (var mapping in _mappings)
            {
                if (_isCCs)
                    incDecCC(mapping, step);
                else if (_isNotes)
                    incDecNote(mapping, step);
            }

            analyzeSelection(true);
            updateMenus(false, true);
        }

        public bool CanIncDec(int step = 0)
        {
            if (!IsGenericMidi)
                return false;

            if (_hasCombo || !(_isCCs || _isNotes))
                return false;

            if (step < 0)
                return _min + step >= 0;
            else
                return 127 >= _max + step;
        }


        ////////////
        // pestrela: inc channel
        public void IncDecCh(int step)
        {
            foreach (var mapping in _mappings)
            {
                if (_isCCs)
                    incDecCC_Ch(mapping, step);
                else if (_isNotes)
                    incDecNote_Ch(mapping, step);
            }

            analyzeSelection(true);
            updateMenus(false, true);
        }

        public bool CanIncDecCh(int step = 0)
        {
            if (!_allBound)
                return false;

            if (!IsGenericMidi)
                return false;

            if (_hasCombo || !(_isCCs || _isNotes))
                return false;

            if ((_min_ch + step) < 1)
                return false;
            else if ((_max_ch + step) > 16)
                return false;
            else
                return true;
            
        }


        private void analyzeSelection(bool allowOverrideNote = false)
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

                notes = parts.Select(p => (p != null) ? p.Note : null).Distinct();

                _selectedNotes = notes.Where(n => n != null).OrderBy(n => n).Cast<object>().ToList();

                _allBound = notes.All(n => n != null);
                _isCCs = _allBound && notes.All(n => n.Contains("CC"));
                _isNotes = _allBound && notes.All(n => n.Contains("Note"));
                _hasCombo = _allBound && notes.Any(n => n.Contains("+"));

                if (_isCCs)
                {
                    var ccs = notes.Select(n => Int32.Parse(n.Split('.').Last()));
                    _min = ccs.Min();
                    _max = ccs.Max();
                }
                else if (_isNotes)
                {
                    var keyConverter = new MidiLib.Utils.KeyConverter();
                    var keys = notes.Select(n => keyConverter.ToKeyIPN(n.Split('.').Last()));
                    _min = keys.Min();
                    _max = keys.Max();
                }
                else
                {
                    _min = 0;
                    _max = 127;
                }

                // pestrela: check we can inc/dec the channels
                var channels2 = channels.Select(n => n.Substring(2, 2));
                var channels3 = channels2.Select(n => Int32.Parse(n));
                if (_allBound)
                {
                    _min_ch = channels3.Min();
                    _max_ch = channels3.Max();
                }
                else
                {
                    _min_ch = 0;
                    _max_ch = 0;
                }

                _variousChannels = channels.Count() > 1;
                if (!_variousChannels && String.IsNullOrEmpty(_channel))
                    setChannel(channels.Single());
            }
            else
            {
                _selectedNotes = _proprietaryDefinitions.Where(n => notes.Contains(n.Note)).DistinctBy(d => d.Note).OrderBy(d => d.Note).Cast<object>().ToList();
            }


            // pestrea: why is this duplicated?
            _variousNotes = notes.Count() > 1;
            if (!_variousNotes && (String.IsNullOrEmpty(_note) || allowOverrideNote))
                setNote(notes.Single());
                       
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
                        tmpDefinition = AGenericMidiDefinition.Parse(mapping.Command.MappingType, expression);
                }
                else
                {
                    if (tmpDefinition != null && mapping.Command.MappingType != tmpDefinition.Type)
                        tmpDefinition = _proprietaryDefinitions.First(p => p.Type == mapping.Command.MappingType && p.Note == tmpDefinition.Note);
                }

                mapping.SetBinding(tmpDefinition);
            }

            analyzeSelection();
            updateMenus();
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

        private void incDecCC(MappingViewModel mapping, int step)
        {
            var oldBinding = mapping.MidiBinding as ControlChangeMidiDefinition;
            
            var oldCC = oldBinding.Cc;
            var newCC = oldCC + step;

            var new_CC_def = new ControlChangeMidiDefinition(oldBinding.Type, oldBinding.Channel, newCC);

            mapping.SetBinding(new_CC_def);
        }

        private void incDecNote(MappingViewModel mapping, int step)
        {
            var oldBinding = mapping.MidiBinding as NoteMidiDefinition;

            var keyConverter = new MidiLib.Utils.KeyConverter();
            var oldKey = keyConverter.ToKeyIPN(oldBinding.KeyText);
            int newKey = oldKey + step;

            mapping.SetBinding(new NoteMidiDefinition(oldBinding.Type, oldBinding.Channel, keyConverter.GetKeyTextIPN(newKey)));
        }


        // pestrela: manipulate channel
        // private String incDec_channel(String oldCh, int step)
        private int incDec_channel(int oldCh, int step)
        {
            return oldCh + step;
        }


        private void incDecCC_Ch(MappingViewModel mapping, int step)
        {
            var oldBinding = mapping.MidiBinding as ControlChangeMidiDefinition;

            var oldCh = oldBinding.Channel;
            var newCh = incDec_channel(oldCh, step);

            mapping.SetBinding(new ControlChangeMidiDefinition(oldBinding.Type, newCh, oldBinding.Cc));
        }

        private void incDecNote_Ch(MappingViewModel mapping, int step)
        {
            var oldBinding = mapping.MidiBinding as NoteMidiDefinition;

            var oldCh = oldBinding.Channel;
            var newCh = incDec_channel(oldCh, step);
            
            mapping.SetBinding(new NoteMidiDefinition(oldBinding.Type, newCh, oldBinding.KeyText));
        }

        ////////////


        private void applyMidiRange()
        {
            var templateBinding = _mappings.First().MidiBinding as AGenericMidiDefinition;

            var isCC = templateBinding is ControlChangeMidiDefinition;

            var keyConverter = new MidiLib.Utils.KeyConverter();

            int number = isCC ? (templateBinding as ControlChangeMidiDefinition).Cc : keyConverter.ToKeyIPN((templateBinding as NoteMidiDefinition).KeyText);

            AGenericMidiDefinition newBinding;
            foreach (var m in _mappings)
            {
                if (m.MidiBinding != null && m.MidiBinding.Equals(templateBinding))
                    continue;

                number++;

                if (isCC)
                    newBinding = new ControlChangeMidiDefinition(templateBinding.Type, templateBinding.Channel, number);
                else
                    newBinding = new NoteMidiDefinition(templateBinding.Type, templateBinding.Channel, keyConverter.GetKeyTextIPN(number));

                m.SetBinding(newBinding);
            }

            analyzeSelection();
            updateMenus();
        }

        private bool canApplyMidiRange()
        {
            if (!IsGenericMidi)
                return false;

            var firstBinding = _mappings.First().MidiBinding;
            if (firstBinding == null || string.IsNullOrEmpty(firstBinding.Note))
                return false;

            var note = firstBinding.Note;

            var isCC = note.Contains("CC");
            var isNote = note.Contains("Note");
            var isCombo = note.Contains("+");

            if (isCombo || !(isCC || isNote))
                return false;

            var keyConverter = new MidiLib.Utils.KeyConverter();

            int number = 127;
            string suffix = note.Split('.').Last();
            if (isCC)
                number = int.Parse(suffix);
            else if (isNote)
                number = keyConverter.ToKeyIPN(suffix);

            int count = _mappings.Count();
            if (number + count > 127)
                return false;

            return true;
        }


        #region Menus

        private void updateMenus(bool channels = true, bool notes = true)
        {
            if (IsGenericMidi)
            {
                if (channels)
                    updateChannelsMenu(_selectedChannels);

                if (notes)
                    updateIncDecMenu();
            }

            if (notes)
                updateNotesMenu(_selectedNotes);
        }

        #region Channels

        private void selectChannel(MenuItemViewModel item)
        {
            Channel = item.Tag.ToString();
        }

        private IEnumerable<MenuItemViewModel> generateChannelsMenu()
        {
            return Enumerable.Range(1, 16).Select(c =>
            {
                var str = String.Format("Ch{0:00}", c);
                return new MenuItemViewModel { Text = str, Tag = str };
            });
        }

        private void updateChannelsMenu(IEnumerable<string> selectedChannels)
        {
            if (!_selectedChannels.Where(c => c != _channel).Any())
            {
                // if we have a single channel, remove the seperator
                if (ChannelsMenu.Contains(_selectedChannelsMenuItem))
                {
                    ChannelsMenu.Remove(SEPARATOR);
                    ChannelsMenu.Remove(_selectedChannelsMenuItem);
                }
                
                return;
            }

            // else, create the selected channel options, one per channel. This list is already of unique channels
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

        private IEnumerable<MenuItemViewModel> generateIncDecMenu()
        {
            List<int> steps = Enumerable.Range(1, 16).ToList();
            int count = _mappings.Count();
            if (!steps.Contains(count))
            {
                steps.Add(count);
                steps.Sort();
            }

            var menu = new ObservableCollection<MenuItemViewModel> { MenuItemViewModel.Separator };

            string suffix;
            MenuItemViewModel item;
            foreach (var step in steps)
            {
                suffix = (step == count) ? "\t[selection count]" : String.Empty;

                item = new MenuItemViewModel { Text = String.Format("\uFF0B{0:00}{1}", step, suffix), Tag = step };
                menu.Insert(0, item);

                item = new MenuItemViewModel { Text = String.Format("\uFF0D{0:00}{1}", step, suffix), Tag = -step };
                menu.Add(item);
            }

            return menu;
        }

        private void updateIncDecMenu()
        {
            foreach (var item in IncDecMenu)
            {
                if (item.IsSeparator)
                    continue;

                item.IsEnabled = CanIncDec((int)item.Tag);
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
