using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Conditions
{
    public abstract class ACondition
    {
        private readonly System.Reflection.PropertyInfo _valueProperty;
        protected readonly ConditionNumber _number;
        internal readonly Format.MappingSettings RawSettings;
        
        public int Id { get; private set; }

        public ConditionNumber Number { get { return _number; } }

        public string Name { get; private set; }

        public TargetType Target { get; private set; }

        public IReadOnlyDictionary<MappingTargetDeck, string> AssignmentOptions { get; private set; }

        public MappingTargetDeck Assignment
        {
            get { return (_number == ConditionNumber.One) ? RawSettings.ConditionOneTarget : RawSettings.ConditionTwoTarget; }
            set
            {
                if (_number == ConditionNumber.One)
                    RawSettings.ConditionOneTarget = value;
                else
                    RawSettings.ConditionTwoTarget = value;
            }
        }


        internal ACondition(int id, string name, TargetType target, Format.MappingSettings rawSettings, ConditionNumber number)
        {
            RawSettings = rawSettings;
            _number = number;

            Id = id;
            Name = name;
            Target = target;

            _valueProperty = GetType().GetProperty("Value");

            updateAssignmentOptions();
            syncSettings();
        }


        public Dictionary<object, string> GetValueOptions()
        {
            var valueDict = new Dictionary<object, string>();
            Type t = GetType();
            while (!t.IsGenericType && t.BaseType != typeof(object))
                t = t.BaseType;
            if (t.IsGenericType)
            {
                var genericArgumentType = t.GenericTypeArguments.First();
                if (genericArgumentType.IsEnum)
                {
                    var allVals = Enum.GetValues(genericArgumentType).Cast<Enum>();
                    foreach (var val in allVals)
                        valueDict.Add(val, val.ToDescriptionString());
                }
                else if (genericArgumentType == typeof(int))
                {
                    var val = GetValue();
                    if (val != null)
                        valueDict.Add(val, val.ToString());
                }
            }
            return valueDict;
        }

        public object GetValue()
        {
            if (_valueProperty != null)
                return _valueProperty.GetValue(this);
            return null;
        }

        public void SetValue(object value)
        {
            if (_valueProperty != null)
                _valueProperty.SetValue(this, value);
        }

        public ACondition Copy(ConditionNumber number)
        {
            var copy = All.KnownConditions[Id].Create(new MappingSettings(), number);
            copy.Assignment = Assignment;
            copy.SetValue(GetValue());
            return copy;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ACondition other = obj as ACondition;
            if (other == null)
                throw new ArgumentException();

            var val = GetValue();
            var valOther = other.GetValue();

            bool equal = Id.Equals(other.Id) && Assignment.Equals(other.Assignment);
            if (val != null)
                equal &= val.Equals(valOther);

            return equal;
        }

        public override int GetHashCode()
        {
            var val = GetValue();
            return new { Id, Assignment, val }.GetHashCode();
        }

        public override string ToString()
        {
            var val = GetValue();
            return String.Format("{0}{1}", 
                Name, 
                (Target != TargetType.Global) ? " [" + AssignmentOptions[Assignment] + "]" : String.Empty
                );
        }


        internal void Reset()
        {
            if (_number == ConditionNumber.One)
            {
                RawSettings.ConditionOneId = 0;
                RawSettings.ConditionOneTarget = 0;
                RawSettings.ConditionOneValue = new byte[4];
            }
            else
            {
                RawSettings.ConditionTwoId = 0;
                RawSettings.ConditionTwoTarget = 0;
                RawSettings.ConditionTwoValue = new byte[4];
            }
        }


        protected virtual void syncSettings()
        {
            if (_number == ConditionNumber.One)
            {
                if (RawSettings.ConditionOneId != Id)
                {
                    RawSettings.ConditionOneId = Id;
                    RawSettings.ConditionOneTarget = 0;
                }
            }
            else
            {
                if (RawSettings.ConditionTwoId != Id)
                {
                    RawSettings.ConditionTwoId = Id;
                    RawSettings.ConditionTwoTarget = 0;
                }
            }
        }


        private void updateAssignmentOptions()
        {
            var options = new Dictionary<MappingTargetDeck, string>();

            switch (Target)
            {
                case TargetType.Global:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Global");
                    break;
                case TargetType.Track:
                    options.Add(MappingTargetDeck.DeviceTarget, "Device Target");

                    //if ((Conditions)Condition.Id == Conditions.DeckCommon_DeckSizeSelector || (Conditions)Condition.Id == Conditions.DeckCommon_AdvancedPanelToggle)
                    //{
                    //    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Deck A & B");
                    //    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Deck C & D");
                    //}
                    //else
                    {
                        options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Deck A");
                        options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Deck B");
                        options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Deck C");
                        options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Deck D");
                    }
                    break;
                case TargetType.Remix:
                    options.Add(MappingTargetDeck.DeviceTarget, "Device Target");
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Remix Deck 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Remix Deck 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Remix Deck 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Remix Deck 4");
                    break;
                case TargetType.FX:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "FX Unit 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "FX Unit 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "FX Unit 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "FX Unit 4");
                    break;
                case TargetType.Slot:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Remix Deck 1 - Slot 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Remix Deck 1 - Slot 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Remix Deck 1 - Slot 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Remix Deck 1 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck2Slot1, "Remix Deck 2 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck2Slot2, "Remix Deck 2 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck2Slot3, "Remix Deck 2 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck2Slot4, "Remix Deck 2 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck3Slot1, "Remix Deck 3 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck3Slot2, "Remix Deck 3 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck3Slot3, "Remix Deck 3 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck3Slot4, "Remix Deck 3 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck4Slot1, "Remix Deck 4 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck4Slot2, "Remix Deck 4 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck4Slot3, "Remix Deck 4 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck4Slot4, "Remix Deck 4 - Slot 4");
                    break;
                default:
                    options = Enum.GetValues(typeof(MappingTargetDeck)).Cast<MappingTargetDeck>().ToDictionary(d => d, d => d.ToString());
                    break;
            }
            AssignmentOptions = options;
        }
    }
}
