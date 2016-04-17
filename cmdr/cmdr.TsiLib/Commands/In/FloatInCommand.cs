using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class FloatInCommand<T> : ANumericValueInCommand<float> where T : FloatRange, new()
    {
        internal FloatInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, new T(), rawSettings)
        {
            RawSettings.ValueUIType = ValueUIType.Slider;
        }


        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Relative, MappingInteractionMode.Direct, MappingInteractionMode.Increment, MappingInteractionMode.Decrement, MappingInteractionMode.Reset };
            }
        }

        private IParser<float> _parser;
        protected override IParser<float> Parser
        {
            get { return _parser ?? (_parser = new FloatParser()); }
        }
    }
}
