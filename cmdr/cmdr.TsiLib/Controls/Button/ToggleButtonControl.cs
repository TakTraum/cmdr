using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public class ToggleButtonControl : AButtonControl
    {
        internal ToggleButtonControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Toggle, MappingInteractionMode.Hold, MappingInteractionMode.Direct };
            }
        }
    }
}
