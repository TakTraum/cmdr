using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.FaderOrKnob
{
    public abstract class FaderOrKnobControl : AControl
    {
        internal FaderOrKnobControl(ACommand command)
            : base(MappingControlType.FaderOrKnob, command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { 
                    MappingInteractionMode.Relative,
                    MappingInteractionMode.Direct
                };
            }
        }

        public static FaderOrKnobControl FromCommand(ACommand command)
        {
            switch (command.InteractionMode)
            {
                case MappingInteractionMode.Direct:
                    return new DirectFaderOrKnobControl(command);
                case MappingInteractionMode.Relative:
                    return new RelativeFaderOrKnobControl(command);
                default:
                    break;
            }
            return null;
        }
    }
}
