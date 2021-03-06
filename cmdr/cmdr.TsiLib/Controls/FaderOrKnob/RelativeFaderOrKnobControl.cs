﻿using System;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.FaderOrKnob
{
    public class RelativeFaderOrKnobControl : FaderOrKnobControl
    {
        /// <summary>
        /// Rotary Sensitivity in percent. 0% to 300%.
        /// </summary>
        public int RotarySensitivity { get { return (int)Math.Round(_command.RawSettings.RotarySensitivity * 20f); } set { _command.RawSettings.RotarySensitivity = value/20f; } }
        
        /// <summary>
        /// Rotary Acceleration in percent. 0% to 100%.
        /// </summary>
        public int RotaryAcceleration { get { return (int)Math.Round(_command.RawSettings.RotaryAcceleration * 100f); } set { _command.RawSettings.RotaryAcceleration = value/100f; } }


        internal RelativeFaderOrKnobControl(ACommand command)
            : base(command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Relative };
            }
        }
    }
}
