using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class TriggerInCommand : ACommand
    {
        internal TriggerInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, MappingType.In, rawSettings)
        {

        }


        public override bool HasValueUI
        {
            get { return false; }
        }

        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Trigger };
            }
        }
    }
}
