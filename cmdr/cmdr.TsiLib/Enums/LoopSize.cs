using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum LoopSize
    {
        [Description("1/32")]
        _1_32 = 0,

        [Description("1/16")]
        _1_16 = 1,

        [Description("1/8")]
        _1_8 = 2,

        [Description("1/4")]
        _1_4 = 3,

        [Description("1/2")]
        _1_2 = 4,

        [Description("1")]
        _1 = 5,

        [Description("2")]
        _2 = 6,

        [Description("4")]
        _4 = 7,

        [Description("8")]
        _8 = 8,

        [Description("16")]
        _16 = 9,

        [Description("32")]
        _32 = 10
    }
}
