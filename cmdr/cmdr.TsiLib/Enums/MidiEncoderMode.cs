using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    internal enum EncoderMode
    {
        [Description("3Fh/41h")]
        _3Fh_41h = 0,
        
        [Description("7Fh/01h")]
        _7Fh_01h = 1,
        
        Proprietary = 3
    };

    public enum MidiEncoderMode
    {
        [Description("3Fh/41h")]
        _3Fh_41h = 0,

        [Description("7Fh/01h")]
        _7Fh_01h = 1,
    };
}
