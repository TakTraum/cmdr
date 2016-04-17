using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public abstract class ADecIncButtonControl : AButtonControl
    {
        public bool AutoRepeat { get { return _command.RawSettings.AutoRepeat; } set { _command.RawSettings.AutoRepeat = value; } }


        internal ADecIncButtonControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Decrement, MappingInteractionMode.Increment };
            }
        }
    }
}
