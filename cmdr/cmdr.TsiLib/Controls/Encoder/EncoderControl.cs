using System;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Encoder
{
    public class EncoderControl : AControl
    {
        /// <summary>
        /// Rotary Sensitivity in percent. 0% to 300%.
        /// </summary>
        public int RotarySensitivity { get { return (int)Math.Round(_command.RawSettings.RotarySensitivity * 20f); } set { _command.RawSettings.RotarySensitivity = value / 20f; } }

        /// <summary>
        /// Rotary Acceleration in percent. 0% to 100%.
        /// </summary>
        public int RotaryAcceleration { get { return (int)Math.Round(_command.RawSettings.RotaryAcceleration * 100f); } set { _command.RawSettings.RotaryAcceleration = value / 100f; } }


        public MidiEncoderMode EncoderMode
        {
            get {
                //var a = _command;
                //var b = a.RawSettings;

                var ret = _command.RawSettings.EncoderMode2;
                return ret;
            }

            set {
                _command.RawSettings.EncoderMode2 = value;
            }
        }

        internal EncoderControl(ACommand command)
            : base(MappingControlType.Encoder, command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get { return new[] { MappingInteractionMode.Relative, MappingInteractionMode.Direct }; }
        }
    }
}
