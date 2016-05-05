using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Parsers;
using System;

namespace cmdr.TsiLib.Conditions
{
    public abstract class AValueCondition<T> : ACondition
    {
        public T Value
        {
            get { return Parser.DecodeValue((_number == ConditionNumber.One) ? RawSettings.ConditionOneValue : RawSettings.ConditionTwoValue); }
            set
            {
                if (_number == ConditionNumber.One)
                    RawSettings.ConditionOneValue = Parser.EncodeValue(value);
                else
                    RawSettings.ConditionTwoValue = Parser.EncodeValue(value);
            }
        }

        protected abstract IParser<T> Parser { get; }


        internal AValueCondition(int id, string name, TargetType target, Format.MappingSettings rawSettings, ConditionNumber number)
            : base(id, name, target, rawSettings, number)
        {

        }


        public override string ToString()
        {
            return String.Format("{0}={1}",
                base.ToString(),
                (Value != null) ? Value.ToString() : "?"
                );
        }


        protected override void syncSettings()
        {
            base.syncSettings();
            if (_number == ConditionNumber.One && RawSettings.ConditionOneValue == null)
                Value = getDefaultValue();
            else if (_number == ConditionNumber.Two && RawSettings.ConditionTwoValue == null)
                Value = getDefaultValue();
        }

        protected abstract T getDefaultValue();
    }
}
