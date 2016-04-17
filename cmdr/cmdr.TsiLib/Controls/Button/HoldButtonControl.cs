using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public class HoldButtonControl : AButtonControl
    {
        internal HoldButtonControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Hold };
            }
        }
    }
}
