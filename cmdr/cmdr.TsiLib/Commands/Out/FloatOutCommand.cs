using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class FloatOutCommand<T> : ANumericValueOutCommand<float> where T : FloatRange, new()
    {
        internal FloatOutCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, new T(), rawSettings)
        {

        }


        private IParser<float> _parser;
        protected override IParser<float> Parser
        {
            get { return _parser ?? (_parser = new FloatParser()); }
        }
    }
}
