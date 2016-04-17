using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum MidiEncoderMode
    {
        [Description("3Fh/41h")]
        _3Fh_41h = 0,
        
        [Description("7Fh/01h")]
        _7Fh_01h = 1,
        
        Unknown = 3, // TODO: NI Controller?
    };
}
