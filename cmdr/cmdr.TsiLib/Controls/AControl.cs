using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls
{
    public abstract class AControl
    {
        protected ACommand _command;

        public MappingControlType Type { get; private set; }
        public bool Invert { get { return _command.RawSettings.Invert; } set { _command.RawSettings.Invert = value; } }


        internal AControl(MappingControlType type, ACommand command)
        {
            Type = type;
            _command = command;

            if (type != MappingControlType.LED && command.RawSettings.RotarySensitivity == 0)
                command.RawSettings.RotarySensitivity = 5f;
        }


        public abstract MappingInteractionMode[] AllowedInteractionModes { get; }
    }
}
