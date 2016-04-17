using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum MappingControlType
    {
        Button = 0,

        [Description("Fader / Knob")]
        FaderOrKnob = 1,
        
        Encoder = 2,
        
        LED = 65535,
    }
}
