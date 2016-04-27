using ChangeTracking;
using cmdr.TsiLib;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using SettingControlLibrary.SettingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;


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
            set { Command.Assignment = value; raisePropertyChanged("AssignmentExpression"); IsChanged = true; }
        }        

        public string Interaction { get { return String.Format("{0} - {1}", Command.ControlType.ToDescriptionString(), Command.InteractionMode.ToDescriptionString()); } }

        public string ConditionExpression { get { return getConditionExpression(); } }

        public string Comment
        {
            get { return _mapping.Comment; }
            set { _mapping.Comment = value; raisePropertyChanged("Comment"); IsChanged = true; }
        }

        public ACondition Condition1 { get { return _mapping.Condition1; } }

        public ACondition Condition2 { get { return _mapping.Condition2; } }

        public ACommand Command { get { return _mapping.Command; } }

        public AMidiDefinition MidiBinding { get { return _mapping.MidiBinding;} }

        public string MappedTo { get { return (_mapping.MidiBinding != null) ? _mapping.MidiBinding.Note : String.Empty; } }

        public bool CanOverrideFactoryMap { get { return _mapping.CanOverrideFactoryMap; } }

        public bool OverrideFactoryMap
        {
            get { return _mapping.OverrideFactoryMap; }
            set { _mapping.OverrideFactoryMap = value; raisePropertyChanged("OverrideFactoryMap"); IsChanged = true; }
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

        public void UpdateInteraction()
        {
            raisePropertyChanged("Interaction");
            raisePropertyChanged("TraktorCommand");
            IsChanged = true;
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

        private string getConditionExpression()
        {
            var conditions = new[] { Condition1, Condition2 }.Where(c => c != null);
            var expression = String.Join(" AND ", conditions.Select(c => conditionToString(c)));

            var description = ConditionDescriptions.Dict.SingleOrDefault(d => d.Condition == expression);
            if (description != null)
                expression = description.Description;

            return expression;
        }

        private string conditionToString(ACondition condition)
        {
            var val = condition.GetType().GetProperty("Value").GetValue(condition);
            return String.Format("{0}{1}={2}",
                condition.Name,
                (condition.Target != TargetType.Global) ? " [" + condition.AssignmentOptions[condition.Assignment] + "]" : String.Empty,
                (val is Enum) ? ((Enum)val).ToDescriptionString() : val.ToString());
        }


        protected override void Accept()
        {

        }

        protected override void Revert()
        {

        }
    }
}
