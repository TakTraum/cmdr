using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum QuantizeSize
    {
        [Description("1/4")]
        _1_4 = -2,

        [Description("1/2")]
        _1_2 = -1,

        [Description("1")]
        _1 = 0,

        [Description("2")]
        _2 = 1,

        [Description("4")]
        _4 = 2,

        [Description("8")]
        _8 = 3,

        [Description("16")]
        _16 = 4,

        [Description("32")]
        _32 = 5
    }
}
