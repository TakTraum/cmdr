using System;
using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class IntOutCommand<T> : ANumericValueOutCommand<Int32> where T : IntRange, new()
    {
        internal IntOutCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, new T(), rawSettings)
        {

        }


        private IParser<int> _parser;
        protected override IParser<int> Parser
        {
            get { return _parser ?? (_parser = new IntParser()); }
        }
    }
}
