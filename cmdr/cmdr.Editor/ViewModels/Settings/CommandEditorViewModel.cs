using cmdr.Editor.Views;
using cmdr.Editor.Views.CommandViews;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using SettingControlLibrary.SettingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace cmdr.Editor.ViewModels.Settings
{
    public class CommandEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;
        private readonly ACommand _command;

        private Dictionary<Setting, System.Reflection.PropertyInfo> _propertyDict;


        #region Assignment

        private Dictionary<MappingTargetDeck, string> _assignmentOptions;
        public Dictionary<MappingTargetDeck, string> AssignmentOptions
        {
            get { return _assignmentOptions; }
        }

        private bool _isAssignmentEnabled;
        public bool IsAssignmentEnabled
        {
            get { return _isAssignmentEnabled; }
        }

        private MappingTargetDeck _assignment;
        public MappingTargetDeck Assignment
        {
            get
            {
                return _assignment;
            }
            set
            {
                _assignment = value;

                foreach (var mvm in _mappings)
                    mvm.Assignment = _assignment;

                raisePropertyChanged("Assignment");
            }
        }

        #endregion

        private bool _isControlEnabled;
        public bool IsControlEnabled
        {
            get { return _isControlEnabled; }
        }

        public Dictionary<MappingControlType, string> ControlTypeOptions
        {
            get
            {
                if (_command != null)
                    return _command.ControlTypeOptions;
                return new Dictionary<MappingControlType, string>();
            }
        }

        public MappingControlType ControlType
        {
            get
            {
                if (_command != null)
                    return _command.ControlType;
                return (MappingControlType)(-1);
            }
            set
            {
                if (_command == null)
                    return;
                _command.ControlType = value; raisePropertyChanged("ControlType"); raisePropertyChanged("ControlInteractionOptions"); raisePropertyChanged("InteractionMode"); updateContent(); updateMapping();
            }
        }

        public Dictionary<MappingInteractionMode, string> ControlInteractionOptions
        {
            get
            {
                if (_command != null)
                    return _command.ControlInteractionOptions;
                return new Dictionary<MappingInteractionMode, string>();
            }
        }

        public MappingInteractionMode InteractionMode
        {
            get
            {
                if (_command != null)
                    return _command.InteractionMode;
                return (MappingInteractionMode)(-1);
            }
            set
            {
                if (_command == null)
                    return;
                _command.InteractionMode = value; raisePropertyChanged("InteractionMode"); updateContent(); updateMapping();
            }
        }

        private CommandView _settingsContent;
        public CommandView SettingsContent
        {
            get { return _settingsContent; }
            set { _settingsContent = value; raisePropertyChanged("SettingsContent"); }
        }

        private ContentControl _controlSettingsContent;
        public ContentControl ControlSettingsContent
        {
            get { return _controlSettingsContent; }
            set { _controlSettingsContent = value; raisePropertyChanged("ControlSettingsContent"); }
        }


        private CommandEditorViewModel(IEnumerable<MappingViewModel> mappings, Dictionary<MappingTargetDeck, string> assignmentOptions, MappingTargetDeck assignment, ACommand command)
        {
            _mappings = mappings;

            if (assignmentOptions != null)
            {
                _isAssignmentEnabled = true;
                _assignmentOptions = assignmentOptions;
                _assignment = assignment;
            }

            if (command != null)
            {
                _isControlEnabled = true;
                _command = command;
                updateContent();
            }
        }


        private void updateContent()
        {
            Type t = _command.GetType();
            if (_command.HasValueUI)
            {
                if (t.InheritsOrImplements(typeof(EnumInCommand<>)))
                    SettingsContent = new EnumCommandView(_command);
                else if (t.InheritsOrImplements(typeof(FloatInCommand<>)))
                    SettingsContent = new FloatCommandView(_command);
                else if (t.InheritsOrImplements(typeof(IntInCommand<>)))
                    SettingsContent = new IntCommandView(_command);
            }
            else if (_command.ControlType == MappingControlType.LED)
            {
                if (t.InheritsOrImplements(typeof(EffectSelectorOutCommand)))
                    SettingsContent = new EffectSelectorOutCommandView(_command);
                else if (t.InheritsOrImplements(typeof(EnumOutCommand<>)))
                    SettingsContent = new EnumOutCommandView(_command);
                else if (t.InheritsOrImplements(typeof(FloatOutCommand<>)))
                    SettingsContent = new FloatOutCommandView(_command);
                else if (t.InheritsOrImplements(typeof(IntOutCommand<>)))
                    SettingsContent = new IntOutCommandView(_command);
            }
            else
                SettingsContent = null;

            if (SettingsContent != null)
                SettingsContent.ValueChanged += (s, e) => onValueChanged();

            var settings = getSettings();
            var controls = settings.Select(setting => SettingControlLibrary.SettingControlFactory.Create(setting));
            ControlSettingsContent = new SettingsEditor(controls);
        }


        private List<Setting> getSettings()
        {
            _propertyDict = new Dictionary<Setting, System.Reflection.PropertyInfo>();

            string[] ignored = { "Type", "AllowedInteractionModes" };

            var detailsSettings = new List<Setting>();

            Type t = _command.Control.GetType();
            var props = t.GetProperties().Where(p => !ignored.Contains(p.Name));

            int i = 0;
            Setting s = null;
            string name;
            foreach (var p in props)
            {
                Type targetType = p.PropertyType;
                name = p.Name;

                switch (name)
                {
                    case "Invert":
                    case "Blend":
                        s = new BoolSetting(i++, name + ":");
                        break;
                    case "SoftTakeOver":
                        s = new BoolSetting(i++, "Soft Takeover:");
                        break;
                    case "AutoRepeat":
                        s = new BoolSetting(i++, "Auto Repeat:");
                        break;
                    case "RotaryAcceleration":
                        s = new IntSetting(i++, "Rotary Acceleration:", 0, 100);
                        break;
                    case "RotarySensitivity":
                        s = new IntSetting(i++, "Rotary Sensitivity:", 0, 300);
                        break;
                    case "Mode":
                        s = new EnumSetting<MidiEncoderMode>(i++, name + ":");
                        break;
                    case "MidiRangeMin":
                        s = new IntSetting(i++, "MIDI Range Min:", 0, 127);
                        break;
                    case "MidiRangeMax":
                        s = new IntSetting(i++, "MIDI Range Max:", 0, 127);
                        break;
                    case "Resolution":
                        s = new EnumSetting<MappingResolution>(i++, name + ":");
                        break;
                    default:
                        s = null;
                        break;
                }

                if (s != null)
                {
                    object rawValue = p.GetValue(_command.Control, null);

                    s.TryParse(rawValue.ToString());
                    s.AcceptChanges();
                    detailsSettings.Add(s);
                    _propertyDict.Add(s, p);
                    s.PropertyChanged += s_PropertyChanged;
                }
                else
                {
                    // Datatype not supported
                }
            }
            return detailsSettings;
        }

        void s_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var setting = sender as Setting;

            var val = setting.GetType().GetProperty("Value").GetValue(setting);
            _propertyDict[setting].SetValue(_command.Control, val);
            onValueChanged();
        }

        private void onValueChanged()
        {
            raisePropertyChanged("Value");
            updateMapping();
        }

        private void updateMapping()
        {
            foreach (var mapping in _mappings)
                mapping.UpdateInteraction();
        }


        public static CommandEditorViewModel BuildEditor(IEnumerable<MappingViewModel> mappings)
        {
            var count = mappings.Count();
            bool isAny = count > 0;
            bool isMulti = count > 1;

            var assignment = (MappingTargetDeck)(-1);
            bool isAssignmentEnabled = false;
            if (isMulti)
            {
                var targetTypes = mappings.DistinctBy(m => m.TargetType);
                if (targetTypes.Count() == 1)
                {
                    isAssignmentEnabled = true;
                    var commonAssignment = mappings.DistinctBy(m => m.Assignment);
                    if (commonAssignment.Count() == 1)
                        assignment = commonAssignment.First().Assignment;
                }
            }
            else if (isAny)
            {
                isAssignmentEnabled = true;
                assignment = mappings.Single().Assignment;
            }

            Dictionary<MappingTargetDeck, string> assignmentOptions = null;
            if (isAssignmentEnabled)
                assignmentOptions = mappings.First().Command.AssignmentOptions;


            ACommand command = null;
            if (!isMulti && isAny)
                command = mappings.Single().Command;

            if (isAssignmentEnabled || command != null)
                return new CommandEditorViewModel(mappings, assignmentOptions, assignment, command);
            else
                return null;
        }
    }
}
