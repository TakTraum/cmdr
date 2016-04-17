using System;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Conditions
{
    public class IntCondition : AValueCondition<Int32>
    {
        internal IntCondition(int id, string name, TargetType target, Format.MappingSettings rawSettings, ConditionNumber number)
            : base(id, name, target, rawSettings, number)
        {

        }


        private IParser<int> _parser;
        protected override IParser<int> Parser
        {
            get { return _parser ?? (_parser = new IntParser()); }
        }

        protected override int getDefaultValue()
        {
            return 0;
        }
    }
}
