using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public class DirectButtonControl : AButtonControl
    {
        internal DirectButtonControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Direct, MappingInteractionMode.Reset };
            }
        }
    }
}
