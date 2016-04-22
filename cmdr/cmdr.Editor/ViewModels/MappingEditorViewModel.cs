using cmdr.Editor.ViewModels.Comment;
using cmdr.Editor.ViewModels.Conditions;
using cmdr.Editor.ViewModels.MidiBinding;
using cmdr.Editor.ViewModels.Settings;
using System.Collections.Generic;

namespace cmdr.Editor.ViewModels
{
    public class MappingEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;
        private readonly DeviceViewModel _device;

        private CommentEditorViewModel _commentEditor;
        public CommentEditorViewModel CommentEditor { get { return _commentEditor; } }

        private ConditionsEditorViewModel _conditionsEditor;
        public ConditionsEditorViewModel ConditionsEditor { get { return _conditionsEditor; } }


        #region Command

        private bool _isCommandEnabled;
        public bool IsCommandEnabled
        {
            get { return _isCommandEnabled; }
        }

        private CommandEditorViewModel _commandEditor;
        public CommandEditorViewModel CommandEditor { get { return _commandEditor; } }

        #endregion

        #region MidiBinding

        private bool _isBindingEnabled = true;
        public bool IsBindingEnabled
        {
            get { return _isBindingEnabled; }
        }

        private MidiBindingEditorViewModel _midiBindingEditor;
        public MidiBindingEditorViewModel MidiBindingEditor { get { return _midiBindingEditor; } }

        #endregion


        public MappingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappings)
        {
            _device = device;

            _mappings = mappings ?? new List<MappingViewModel>();

            _commentEditor = new CommentEditorViewModel(_mappings);

            _conditionsEditor = new ConditionsEditorViewModel(_mappings);

            _commandEditor = CommandEditorViewModel.BuildEditor(_mappings);
            _isCommandEnabled = _commandEditor != null;

            _midiBindingEditor = MidiBindingEditorViewModel.BuildEditor(_device, _mappings);
            _isBindingEnabled = _midiBindingEditor != null;
        }
    }
}
