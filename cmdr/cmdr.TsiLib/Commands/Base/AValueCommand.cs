using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public abstract class AValueCommand<T> : ACommand
    {
        protected abstract IParser<T> Parser { get; }


        internal AValueCommand(int id, string name, Enums.TargetType target, MappingType mappingType, MappingSettings rawSettings) 
            : base(id, name, target, mappingType, rawSettings)
        {

        }


        public override bool HasValueUI
        {
            get { return ControlType == MappingControlType.Button && (InteractionMode == MappingInteractionMode.Direct || InteractionMode == MappingInteractionMode.Hold); }
        }
    }
}
