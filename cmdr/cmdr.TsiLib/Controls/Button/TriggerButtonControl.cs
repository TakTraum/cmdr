using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public class TriggerButtonControl : AButtonControl
    {
        internal TriggerButtonControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Trigger };
            }
        }
    }
}
