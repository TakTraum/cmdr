using System;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Conditions
{
    public class EnumCondition<T> : AValueCondition<T> where T : struct, IConvertible
    {
        private IParser<T> _parser;
        protected override IParser<T> Parser
        {
            get { return _parser ?? (_parser = new EnumParser<T>()); }
        }

        
        internal EnumCondition(int id, string name, TargetType target, Format.MappingSettings rawSettings, ConditionNumber number)
            : base(id, name, target, rawSettings, number)
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T must be an Enumeration type.");
        }


        public override string ToString()
        {
            var val = GetValue();
            return String.Format("{0}{1}={2}",
                Name,
                (Target != TargetType.Global) ? " [" + AssignmentOptions[Assignment] + "]" : String.Empty,
                (Value as Enum).ToDescriptionString()
                );
        }

        protected override T getDefaultValue()
        {
            return EnumParser<T>.AllValues.Min();
        }
    }
}
