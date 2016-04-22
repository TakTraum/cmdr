using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum FluxState
    {
        Off = 0,
        On = 1,
        [Description("In Action")]
        InAction = 2
    }
}
