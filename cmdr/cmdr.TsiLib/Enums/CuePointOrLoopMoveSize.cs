using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum CuePointOrLoopMoveSize
    {
        UltraFine = 0,
        
        Fine = 1,

        [Description("1/16")]
        _1_16 = 2,

        [Description("1/8")]
        _1_8 = 3,

        [Description("1/4")]
        _1_4 = 4,

        [Description("1/2")]
        _1_2 = 5,

        [Description("1")]
        _1 = 6,

        [Description("2")]
        _2 = 7,

        [Description("4")]
        _4 = 8,

        [Description("8")]
        _8 = 9,

        [Description("16")]
        _16 = 10,

        [Description("32")]
        _32 = 11,
        
        Loop = 12
    }
}
