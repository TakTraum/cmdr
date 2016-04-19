using ChangeTracking;
using cmdr.Editor.Utils;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions;
using cmdr.TsiLib.MidiDefinitions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels.MidiBinding
{
    public class MidiBindingEditorViewModel : ViewModelBase
    {
        private readonly DeviceViewModel _device;
        private readonly IEnumerable<MappingViewModel> _mappings;

        private MidiLearner _midiLearner;


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

        private List<string> _channels;
        public List<string> Channels { get { return _channels ?? (_channels = Enumerable.Range(1, 16).Select(i => String.Format("Ch{0:00}", i)).ToList()); } }

        private string _channel;
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; raisePropertyChanged("Channel"); updateBinding(); }
        }

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

                raisePropertyChanged("Note");

                if (IsGenericMidi)
                    updateBinding();
            }
        }

        public ContextMenu NotesMenu { get; private set; }


        #region Commands

        private ICommand _selectNoteCommand;
        public ICommand SelectNoteCommand
        {
            get { return _selectNoteCommand ?? (_selectNoteCommand = new CommandHandler<Button>(showNotesMenu)); }
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


        private MidiBindingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappings, AMidiDefinition binding)
        {
            _mappings = mappings;
            _device = device;

            if (IsGenericMidi)
            {
                NotesMenu = generateGenericMidiMenu();
                _midiLearner = new MidiLearner((signal) =>
                {
                    toggleLearn();

                    _channel = String.Format("Ch{0:00}", signal.Channel);
                    raisePropertyChanged("Channel");
                    Note = signal.Note;

                    // workaround to refresh combo and reset button
                    (ComboCommand as CommandHandler).UpdateCanExecuteState();
                    (ResetCommand as CommandHandler).UpdateCanExecuteState();
                });

                if (binding != null)
                {
                    _channel = binding.Note.Substring(0, 4);
                    _note = binding.Note.Replace(_channel + ".", "");
                }
                else
                    _channel = Channels.First();
            }
            else
            {
                _canOverrideFactoryMap = _mappings.Any(m => m.CanOverrideFactoryMap);
                NotesMenu = generateProprietaryMenu();
                if (binding != null)
                    _note = binding.Note;
            }
        }


        private void updateBinding(AMidiDefinition definition = null)
        {
            if (definition == null)
            {
                if (!String.IsNullOrEmpty(_channel) && !String.IsNullOrEmpty(_note))
                {
                    string expression = _channel + "." + _note.Replace("+", "+" + _channel + ".");
                    definition = new GenericMidiDefinition(_mappings.First().Command.MappingType, expression);
                }
            }

            foreach (var mapping in _mappings)
                mapping.SetBinding(definition);
        }

        private void reset()
        {
            if (IsGenericMidi)
            {
                _channel = Channels.First();
                raisePropertyChanged("Channel");
            }
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

            return !(String.IsNullOrEmpty(Note) || Note.Contains("+"));
        }


        #region ContextMenu Notes

        private void showNotesMenu(Button button)
        {
            NotesMenu.PlacementTarget = button;
            NotesMenu.IsOpen = true;
        }

        private ContextMenu generateProprietaryMenu()
        {
            Action<AMidiDefinition> proprietaryAction = new Action<AMidiDefinition>((definition) =>
            {
                // it's ok to set a reference here, as there should be a single definition per note anyway
                updateBinding(definition);
                Note = definition.Note;
            });

            ContextMenu root = new ContextMenu();

            MenuItem menuItemL1 = null;
            MenuItem menuItemL2 = null;
            MenuItem menuItemL3 = null;

            MappingType mType = _mappings.First().Command.MappingType;

            List<AMidiDefinition> defs = getProprietaryDefinitions(_device, mType).OrderBy(d => d.Note).ToList();

            string note;
            foreach (var def in defs)
            {
                note = def.Note;

                var parts = note.Split('.');

                var h1 = parts[0].Trim();
                if (menuItemL1 == null || menuItemL1.Header.ToString() != h1)
                {
                    menuItemL1 = createMenuItem(h1);
                    root.Items.Add(menuItemL1);
                }
                if (parts.Length > 1)
                {
                    var h2 = parts[1].Trim();
                    if (menuItemL2 == null || menuItemL2.Header.ToString() != h2)
                    {
                        menuItemL2 = createMenuItem(h2);
                        menuItemL1.Items.Add(menuItemL2);
                    }
                    if (parts.Length > 2)
                    {
                        var h3 = parts[2].Trim();
                        if (menuItemL3 == null || menuItemL3.Header.ToString() != h3)
                        {
                            menuItemL3 = createMenuItem(h3, new CommandHandler(() => proprietaryAction(def)));
                            menuItemL2.Items.Add(menuItemL3);
                        }
                    }
                    else
                        menuItemL2.Command = new CommandHandler(() => proprietaryAction(def));
                }
                else
                    menuItemL1.Command = new CommandHandler(() => proprietaryAction(def));
            }

            return root;
        }

        private ContextMenu generateGenericMidiMenu()
        {
            Action<string> genericAction = new Action<string>((n) => { Note = n; });

            ContextMenu root = new ContextMenu();

            #region CC

            MenuItem ccMenu = createMenuItem("CC");
            root.Items.Add(ccMenu);
            MenuItem rangeMenu = null;
            int num = 128;
            int parts = 4;
            int part = 0;
            int limit = 0;

            for (int i = 0; i < num; i++)
            {
                if (i == limit)
                {
                    part++;
                    limit = num / parts * part;
                    rangeMenu = createMenuItem(String.Format("{0} - {1}", i, limit-1));
                    ccMenu.Items.Add(rangeMenu);
                }
                rangeMenu.Items.Add(createMenuItem(String.Format("{0:000}", i), new CommandHandler<string>(genericAction), String.Format("CC.{0:000}", i)));
            }

            #endregion

            #region Notes

            List<string> notes = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            MenuItem notesMenu = createMenuItem("Note");
            root.Items.Add(notesMenu);
            MenuItem noteMenu = null;
            foreach (var note in notes)
            {
                noteMenu = createMenuItem(note);
                notesMenu.Items.Add(noteMenu);
                for (int i = -1; i < 10; i++)
                    noteMenu.Items.Add(createMenuItem(i.ToString(), new CommandHandler<string>(genericAction), String.Format("Note.{0}", note + i)));
            }
            #endregion

            // PitchBend
            root.Items.Add(createMenuItem("PitchBend", new CommandHandler<string>(genericAction), "PitchBend"));
            
            return root;
        }

        private MenuItem createMenuItem(string caption, ICommand command = null, object commandParameter = null)
        {
            var item = new MenuItem { Header = caption };
            if (command != null)
                item.Command = command;
            if (commandParameter != null)
                item.CommandParameter = commandParameter;
            return item;
        }

        #endregion


        public static MidiBindingEditorViewModel BuildEditor(DeviceViewModel device, IEnumerable<MappingViewModel> mappings)
        {
            var count = mappings.Count();
            bool isAny = count > 0;
            bool isMulti = count > 1;

            if (isMulti)
            {
                var sameMappingType = mappings.Select(mvm => mvm.Command.MappingType).Distinct();
                if (sameMappingType.Count() == 1)
                {
                    if (mappings.Any(mvm => mvm.MidiBinding == null))
                        return new MidiBindingEditorViewModel(device, mappings, null);
                    else
                    {
                        var sameNote = mappings.Select(mvm => mvm.MidiBinding).Distinct();
                        if (sameNote.Count() == 1)
                            return new MidiBindingEditorViewModel(device, mappings, sameNote.First());
                        else
                            return new MidiBindingEditorViewModel(device, mappings, null);
                    }
                }
            }
            else if (isAny)
            {
                var mvm = mappings.Single();
                return new MidiBindingEditorViewModel(device, mappings, mvm.MidiBinding);
            }
            return null;

        }

        private IEnumerable<AMidiDefinition> getProprietaryDefinitions(DeviceViewModel device, MappingType type)
        {
            Dictionary<string, AMidiDefinition> definitions = new Dictionary<string, AMidiDefinition>();

            // get definitions from current tsi
            definitions = ((type == MappingType.In) ? _device.MidiInDefinitions : _device.MidiOutDefinitions)
                .ToDictionary(e => e.Key, e => e.Value);

            // TODO: addd definitions for proprietary devices from another tsi file (e.g. NI's default mapping)
            //string tsiDefinitionsFile = null;
            //MidiDefinitionFactory.GetInDefinitionsFromTsi(tsiDefinitionsFile);
            //MidiDefinitionFactory.GetOutDefinitionsFromTsi(tsiDefinitionsFile);

            return definitions.Values;
        }

    }
}
