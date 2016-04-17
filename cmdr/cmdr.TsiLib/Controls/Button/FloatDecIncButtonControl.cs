using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public class FloatDecIncButtonControl : ADecIncButtonControl
    {
        public MappingResolution Resolution { get { return _command.RawSettings.Resolution; } set { _command.RawSettings.Resolution = value; } }


        internal FloatDecIncButtonControl(ACommand command)
            : base(command)
        {

        }
    }
}
