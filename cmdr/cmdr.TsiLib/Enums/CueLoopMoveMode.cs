using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum CueLoopMoveMode
    {
        Beatjump = 0,
        
        Loop = 1,
        
        [Description("Loop In")]
        LoopIn = 2,

        [Description("Loop Out")]
        LoopOut = 3
    }
}
