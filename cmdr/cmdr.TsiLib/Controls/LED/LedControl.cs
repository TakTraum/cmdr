using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.LED
{
    public class LedControl<T> : AControl
    {
        // Note: this sequence determines what is presented on screen
        public int MidiRangeMin { get { return _command.RawSettings.LedMinMidiRange; } set { _command.RawSettings.LedMinMidiRange = value; } }

        public int MidiRangeMax { get { return _command.RawSettings.LedMaxMidiRange; } set { _command.RawSettings.LedMaxMidiRange = value; } }

        public bool Blend { get { return _command.RawSettings.LedBlend; } set { _command.RawSettings.LedBlend = value; } }

        internal LedControl(ACommand command)
            : base(MappingControlType.LED, command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get { return new[] { MappingInteractionMode.Output }; }
        }
    }
}
