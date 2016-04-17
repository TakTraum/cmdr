using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.FaderOrKnob
{
    public class DirectFaderOrKnobControl : FaderOrKnobControl
    {
        public bool SoftTakeOver { get { return _command.RawSettings.SoftTakeover; } set { _command.RawSettings.SoftTakeover = value; } }


        internal DirectFaderOrKnobControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Direct };
            }
        }
    }
}
