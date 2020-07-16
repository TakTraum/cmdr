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
        public bool IsKeyboard { get { return _device.IsKeyboard; } }

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

        private List<string> _selectedStrings;
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
        private int ChannelsMenu_shortcuts = 0;

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
                if (ComboMode && !String.IsNullOrEmpty(value)) {
                    _note += "+" + value;
                    ComboMode = false;
                } else {
                    _note = value;
                }
                if (IsGenericMidi) {
                    updateBinding();
                }

                raisePropertyChanged("NoteString");
            }
        }

        public string NoteString
        {
            get { return VariousNotes ? "Various" : String.IsNullOrEmpty(Note) ? "None" : Note; }
        }

        public ObservableCollection<MenuItemViewModel> NotesMenu { get; private set; }

        private int NotesMenu_shortcuts = 0;

        private bool _variousNotes;
        public bool VariousNotes
        {
            get { return _variousNotes; }
        }

        public ObservableCollection<MenuItemViewModel> IncDecMenu { get; private set; }

        private bool _isCCs = false;   // is ONLY CCs
        private bool _isNotes = false;  // is ONLY notes
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

        // fixme: these two commands are the same!
        private ICommand _removeBinding;
        public ICommand RemoveBinding
        {
            get { return _removeBinding ?? (_removeBinding = new CommandHandler(removeBinding));}
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
            get { return _incDecCommand ?? (_incDecCommand = new CommandHandler<MenuItemViewModel>(item => IncDecNumber((int)item.Tag), () => CanIncDecNumber())); }
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

                NotesMenu.Insert(0, SEPARATOR);
                ChannelsMenu.Insert(0, SEPARATOR);

            } else
            {
                _canOverrideFactoryMap = _mappings.Any(m => m.CanOverrideFactoryMap);
                Char split;
                if (IsKeyboard) {
                    split = '+';
                } else {
                    split = '.';
                }
                var tmp = NotesMenuBuilder.Instance.BuildProprietaryMenu(_proprietaryDefinitions.DistinctBy(d => d.Note), split);
                NotesMenu = new ObservableCollection<MenuItemViewModel>(tmp);
            }


            updateMenus();
        }

        private void incDecMapping(MappingViewModel mapping, int step, IncDecWhat what)
        {
            var oldBinding = mapping.MidiBinding as AGenericMidiDefinition;
            if(oldBinding != null)
                mapping.SetBinding(incDecGeneric(oldBinding, step, what));
        }

        public void IncDecMappings(int step, IncDecWhat what)
        {
            foreach (var mapping in _mappings)
            {
                incDecMapping(mapping, step, what);
            }

            analyzeSelection(true);
            updateMenus(false, true);
        }

        public void IncDecNumber(int step)
        {
            IncDecMappings(step, IncDecWhat.Number);
        }

        public bool CanIncDecNumber(int step = 0)
        {
            if (!IsGenericMidi)
                return false;

            return true;

            //_hasCombo
            if ( !(_isCCs || _isNotes))
                return false;

            if (step < 0)
                return _min + step >= 0;
            else
                return 127 >= _max + step;
        }

        public void IncDecChannel(int step)
        {
            IncDecMappings(step, IncDecWhat.Channel);
        }

        public bool CanIncDecChannel(int step = 0)
        {
            if (!IsGenericMidi)
                return false;

            return true;

            if (!_allBound)
                return false;

            //_hasCombo ||
            if ( !(_isCCs || _isNotes))
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
            _selectedStrings = notes.Where(c => c != null).ToList();
            _selectedStrings.Sort();
            _selectedStrings.Reverse();

            if (IsGenericMidi)
            {
                // This is a shorthand for the notes menu
                if (_selectedStrings.Count() == 0) {
                    _selectedStrings.Add("Ch01.Note.C0");
                }

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
                _isNotes = _allBound && notes.All(n => n.Contains("Note"));  //Newer code checks explicitelly for types
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


            // pestrela: why is this duplicated?
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


        // This is when we select a new Channel or Note
        private void updateBinding(AMidiDefinition definition = null)
        {
            //if (_channel == "None")
            //    _channel = null;  // this is to simplify the processing a lot

            string expression;
            AMidiDefinition tmpDefinition;
            bool needs_cleanup = false;
            foreach (var mapping in _mappings)
            {
                expression = null;
                tmpDefinition = definition;

                if (IsGenericMidi)
                {
                    // special HACK follows
                    if (!String.IsNullOrEmpty(_note) && _note[0] == '_') {

                        // remove the initial '_', but keep it for further processing
                        // the expression is already complete - doesn't need manipulations
                        expression = _note.Substring(1);

                    } else if (!String.IsNullOrEmpty(_channel))
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

            if (needs_cleanup) {
                _note = _note.Substring(6);   //this is just to display correctly in the textbox
            };

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

        private void removeBinding()
        {
            reset();
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


        /////// channels
        public enum IncDecWhat
        {
            Channel,
            Number,
        }

        // note: channels do not wrap around on purpose
        private int incDec_channel(int old, int step, IncDecWhat what)
        {
            if (what != IncDecWhat.Channel) {
                step = 0;
            }

            int new_i = old + step;
            if(new_i <= 1) {
                new_i = 1;
            }

            if(new_i > 16) {
                new_i = 16;
            }

            return new_i;
        }

        private int incDec_note(int old, int step, IncDecWhat what)
        {
            if (what != IncDecWhat.Number) {
                step = 0;
            }

            int new_i = old + step;
            if (new_i < 0) {
                new_i = 0;
            }

            if (new_i > 127) {
                new_i = 127;
            }

            return new_i;
        }

        /*
         * Note1: 
         *   "Mapping.MidiBinding" comes without type by some reason. 
         *   Workaound inspects the types explicitelly
         *   We could use AGenericMidiDefinition.Parse but then we had to generte the string by hand
         *
         * Note2: 
         *   "Type" is not C#. Its about traktor "in"/"out"
         * 
         */
        private AGenericMidiDefinition incDecGeneric(AGenericMidiDefinition old, int step, IncDecWhat what)
        {

            if (old is ComboMidiDefinition) {
                var specific = (ComboMidiDefinition)old;

                return new ComboMidiDefinition(
                        incDecGeneric(specific.MidiDefinition1, step, what),
                        incDecGeneric(specific.MidiDefinition2, step, what));
            }

            // at this stage we always have a channel
            int old_channel = old.Channel;
            int new_channel = incDec_channel(old_channel, step, what);

            if (old is NoteMidiDefinition) {
                var specific = (NoteMidiDefinition)old;
                var keyConverter = new MidiLib.Utils.KeyConverter();

                int old_value = keyConverter.ToKeyIPN(specific.KeyText);
                int new_value = incDec_note(old_value, step, what);
                string new_note = keyConverter.GetKeyTextIPN(new_value);

                return new NoteMidiDefinition(old.Type, new_channel, new_note);
            }

            if (old is ControlChangeMidiDefinition) {
                var specific = (ControlChangeMidiDefinition)old;

                int new_value = incDec_note(specific.Cc, step, what);

                return new ControlChangeMidiDefinition(old.Type, new_channel, new_value);
            }

            // this should never be reached!
            return null;
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

            /// check-me
            if (notes || true) {
                updateNotesMenu(_selectedNotes);
            }
        }

        #region Channels

        private void selectChannel(MenuItemViewModel item)
        {
            Channel = item.Tag.ToString();
        }

        private IEnumerable<MenuItemViewModel> generateChannelsMenu()
        {
            var ret = Enumerable.Range(1, 16).Select(c =>
            {
                var str = String.Format("Ch{0:00}", c);
                return new MenuItemViewModel { Text = str, Tag = str };
            });

            // Add a "None" entry to the channels
            //ret.toList().Insert(0, new MenuItemViewModel { Text = "None", Tag = "None" });
            return ret;
        }



        private void updateChannelsMenu(IEnumerable<string> selectedChannels)
        {
            // todo: merge this method with the notes one
            // remove possible previous entries
            while (ChannelsMenu_shortcuts > 0)
            {
                //OutCommands.Add(_separator);
                ChannelsMenu.RemoveAt(0);
                ChannelsMenu_shortcuts -= 1;
            }

            if(_selectedChannels.Count == 0)
            {
                return;
            }

            // else, create the selected channel options, one per channel. This list is already of unique channels
            _selectedChannelsMenuItem.Children = _selectedChannels.Select(c => new MenuItemViewModel { Text = c, Tag = c }).ToList();

            foreach (var item in _selectedChannelsMenuItem.Children)
            {
                ChannelsMenu.Insert(0, item);
                ChannelsMenu_shortcuts += 1;
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
            } else {
                // generic midi
                Note = item.Tag.ToString();
            }
        }

        private void updateNotesMenu(IEnumerable<object> selectedNotes)
        {
            // remove possible previous entries
            while(NotesMenu_shortcuts > 0)
            {
                //OutCommands.Add(_separator);
                NotesMenu.RemoveAt(0);
                NotesMenu_shortcuts -= 1;
            }

            ////
            var hasVariousNotesUnequalNull = (IsGenericMidi && !_selectedNotes.Where(n => n.ToString() != _note).Any()) ||
                (!IsGenericMidi && !_selectedNotes.Cast<AMidiDefinition>().Where(n => n.Note != _note).Any());

            if (selectedNotes.Count() > 0) {
                // todo: do real C# code here
                _selectedNotesMenuItem.Children = NotesMenuBuilder.Instance.BuildSelectedNotesMenu(selectedNotes);
                foreach (var item in _selectedNotesMenuItem.Children) {
                    NotesMenu.Insert(0, item);
                    NotesMenu_shortcuts += 1;
                }
            }

            // note: this is already reverse sorted
            foreach (var st in _selectedStrings) {

                // FIXME: Hack: we use '_' to signifify that it has channel+note
                var item = new MenuItemViewModel { Text = st, Tag = '_' + st };  //special tags start with '_'
                NotesMenu.Insert(0, item);
                NotesMenu_shortcuts += 1;
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

                item.IsEnabled = CanIncDecNumber((int)item.Tag);
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
