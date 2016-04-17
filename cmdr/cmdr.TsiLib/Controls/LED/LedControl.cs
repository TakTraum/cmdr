using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.LED
{
    public class LedControl<T> : AControl
    {
        public bool Blend { get { return _command.RawSettings.LedBlend; } set { _command.RawSettings.LedBlend = value; } }

        /// <summary>
        /// Range Minimum: 0 to 127.
        /// </summary>
        public int MidiRangeMin { get { return _command.RawSettings.LedMinMidiRange; } set { _command.RawSettings.LedMinMidiRange = value; } }

        /// <summary>
        /// Range Maximum: 0 to 127.
        /// </summary>
        public int MidiRangeMax { get { return _command.RawSettings.LedMaxMidiRange; } set { _command.RawSettings.LedMaxMidiRange = value; } }


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
