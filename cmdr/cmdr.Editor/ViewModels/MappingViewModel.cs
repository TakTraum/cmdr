using ChangeTracking;
using cmdr.Editor.Metadata;
using cmdr.Editor.ViewModels.Conditions;
using cmdr.TsiLib;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.TsiLib.Commands.Interpretation;
using System;
using System.Linq;
using System.Text;

namespace cmdr.Editor.ViewModels
{
    public class MappingViewModel : AReversible
    {
        private const int VALUE_TEXT_MAX_PRECISION = 4;

        private readonly Device _device;
        private readonly Mapping _mapping;

        public int Id { get { return _mapping.Id; } }

        public string TraktorCommand { get { return getTraktorCommand(); } }

        public string Type { get { return Command.MappingType.ToString(); } }

        public TargetType TargetType { get { return Command.Target; } }

        public string AssignmentExpression { get { return Command.AssignmentOptions[Assignment]; } }

        public MappingTargetDeck Assignment
        {
            get { return Command.Assignment; }
            set { Command.Assignment = value; raisePropertyChanged("Assignment"); raisePropertyChanged("AssignmentExpression"); IsChanged = true; }
        }        

        public string Interaction { get { return String.Format("{0} - {1}", Command.ControlType.ToDescriptionString(), Command.InteractionMode.ToDescriptionString()); } }

        public string ConditionExpression { get { return Conditions.Name ?? Conditions.ToString(); } }

        public string Comment
        {
            get { return _mapping.Comment; }
            set { _mapping.Comment = value; raisePropertyChanged("Comment"); IsChanged = true; }
        }

        public ConditionTuple Conditions { get { return _mapping.Conditions; } }

        public ACommand Command { get { return _mapping.Command; } }

        public AMidiDefinition MidiBinding { get { return _mapping.MidiBinding;} }

        public string MappedTo
        {
            get
            {
                return (_mapping.MidiBinding != null)
                   ? _mapping.MidiBinding.Note : String.Empty;
            }
        }

        public string MappedTo_OnlyNote
        {
            get
            {
                if (_mapping.MidiBinding == null)
                    return String.Empty;

                String note = _mapping.MidiBinding.Note;
                int len = note.Length;

                int i = note.IndexOf('.');
                if (i == -1)
                {
                    return note;
                }

                String channel = note.Substring(0, i);
                String rest = note.Substring(i + 1); //, len - i);
                String ret = rest + "." + channel;
                return ret;
            }
        }


        public bool CanOverrideFactoryMap { get { return _mapping.CanOverrideFactoryMap; } }

        public bool OverrideFactoryMap
        {
            get { return _mapping.OverrideFactoryMap; }
            set { _mapping.OverrideFactoryMap = value; raisePropertyChanged("OverrideFactoryMap"); IsChanged = true; }
        }

        private MappingMetadata _metadata;
        public MappingMetadata Metadata
        {
            get { return _metadata; }
            set { _metadata = value; raisePropertyChanged("Metadata"); }
        }


        public MappingViewModel(Device device, Mapping mapping)
        {
            _device = device;
            _mapping = mapping;
        }


        public void SetBinding(AMidiDefinition binding)
        {
            var changed = _mapping.SetBinding(_device, binding);
            if (changed)
            {
                raisePropertyChanged("MidiBinding");
                raisePropertyChanged("MappedTo");
                raisePropertyChanged("MappedTo_OnlyNote");
                IsChanged = true;
            }
        }

        public void SetCondition(ConditionNumber number, ConditionProxy proxy)
        {
            var changed = _mapping.SetCondition(number, proxy);
            if (changed)
            {
                if (number == ConditionNumber.One)
                    raisePropertyChanged("Condition1");
                else
                    raisePropertyChanged("Condition2");
                raisePropertyChanged("ConditionExpression");
                IsChanged = true;
            }
        }

        public void SetCondition(ConditionNumber number, ACondition condition)
        {
            var changed = _mapping.SetCondition(number, condition);
            if (changed)
            {
                if (number == ConditionNumber.One)
                    raisePropertyChanged("Condition1");
                else
                    raisePropertyChanged("Condition2");
                raisePropertyChanged("ConditionExpression");
                IsChanged = true;
            }
        }

        public Mapping Copy(bool includeMidiBinding)
        {
            return _mapping.Copy(includeMidiBinding);
        }

        public void hack_encoder_mididefinition()
        {
            AGenericMidiDefinition midiBinding = (this._mapping.MidiBinding as AGenericMidiDefinition);
            if(midiBinding == null)
            {
                return;    //nothing todo. We are not linked to a midi modifier yet
            }

            MidiEncoderMode actual_encoder_mode = _mapping.Command.get_EncoderMode();
            midiBinding.MidiEncoderMode = actual_encoder_mode;
        }


        public void UpdateInteraction()
        {
            hack_encoder_mididefinition();

            raisePropertyChanged("Interaction");
            raisePropertyChanged("TraktorCommand");
            IsChanged = true;
        }

        public void UpdateConditionExpression()
        {
            raisePropertyChanged("ConditionExpression");
        }

        public void ChangeAssignment(MappingTargetDeck assignment)
        {
            if (Conditions.Condition1 != null && Conditions.Condition1.Target == TargetType && Conditions.Condition1.Assignment == Assignment)
            {
                Conditions.Condition1.Assignment = assignment;
                raisePropertyChanged("Conditions");
                raisePropertyChanged("ConditionExpression");
            }

            if (Conditions.Condition2 != null && Conditions.Condition2.Target == TargetType && Conditions.Condition2.Assignment == Assignment)
            {
                Conditions.Condition2.Assignment = assignment;
                raisePropertyChanged("Conditions");
                raisePropertyChanged("ConditionExpression");
            }
        
            Assignment = assignment;
        }

        private string getTraktorCommand()
        {
            StringBuilder sb = new StringBuilder(100);
            sb.Append(Command.Name);

            switch (_mapping.Command.InteractionMode)
            {
                case MappingInteractionMode.Trigger:
                    break;
                case MappingInteractionMode.Toggle:
                    sb.Append(" [Toggle]");
                    break;
                case MappingInteractionMode.Hold:
                    sb.Append(" [Hold]");
                    break;

                case MappingInteractionMode.Direct:
                    break;
                case MappingInteractionMode.Relative:
                    break;
                case MappingInteractionMode.Increment:
                    sb.Append(" ++");
                    break;
                case MappingInteractionMode.Decrement:
                    sb.Append(" --");
                    break;
                case MappingInteractionMode.Reset:
                    sb.Append(" [Reset]");
                    break;
                case MappingInteractionMode.Output:
                    if (_mapping.Command is EffectSelectorOutCommand)
                    {
                        var efCo = _mapping.Command as EffectSelectorOutCommand;
                        sb.AppendFormat(" [{0}]", efCo.AllEffects ? "All Effects" : efCo.ControllerRangeMin.ToDescriptionString());
                        break;
                    }

                    object min = _mapping.Command.GetType().GetProperty("ControllerRangeMin").GetValue(_mapping.Command);
                    object max = _mapping.Command.GetType().GetProperty("ControllerRangeMax").GetValue(_mapping.Command);
                    if (min.GetType().IsEnum)
                    {
                        min = ((Enum)min).ToDescriptionString();
                        max = ((Enum)max).ToDescriptionString();
                    }
                    else if (min.GetType().Equals(typeof(float)))
                    {
                        min = String.Format("{0:G" + VALUE_TEXT_MAX_PRECISION + "}", min);
                        max = String.Format("{0:G" + VALUE_TEXT_MAX_PRECISION + "}", max);
                    }
                    sb.AppendFormat(" [{0}; {1}]", min, max);
                    break;
                default:
                    break;
            }

            if (_mapping.Command.HasValueUI)
            {
                var pi = _mapping.Command.GetType().GetProperty("Value");
                if (pi != null)
                {
                    object value = pi.GetValue(_mapping.Command);
                    if (value.GetType().IsEnum)
                        value = ((Enum)value).ToDescriptionString();
                    sb.AppendFormat(" = " + "{0:G" + VALUE_TEXT_MAX_PRECISION + "}", value);
                }
            }

            return sb.ToString();
        }


        protected override void Accept()
        {

        }

        protected override void Revert()
        {

        }

        public void hack_modifier(KnownCommands new_id)
        {
            var b = this._mapping;

            b.hack_modifier(new_id);


        }


        public override bool Equals(object obj)
        {
            MappingViewModel other = obj as MappingViewModel;
            if (other == null)
                return false;
            return _mapping.Equals(other._mapping);
        }

        public override int GetHashCode()
        {
            return _mapping.GetHashCode();
        }
    }
}
