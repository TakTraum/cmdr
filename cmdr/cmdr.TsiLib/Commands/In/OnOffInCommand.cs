using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public sealed class OnOffInCommand : EnumInCommand<OnOff>
    {
        internal OnOffInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }


        public override bool HasValueUI
        {
            get
            {
                return ControlType == MappingControlType.Button && InteractionMode == MappingInteractionMode.Direct;
            }
        }

        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Toggle, MappingInteractionMode.Hold, MappingInteractionMode.Direct };
            }
        }
    }
}
