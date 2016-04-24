using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum MoveSize
    {
        [Description("-Loop")]
        Minus_Loop = -13,

        [Description("-32")]
        Minus_32 = -12,

        [Description("-16")]
        Minus_16 = -11,

        [Description("-8")]
        Minus_8 = -10,

        [Description("-4")]
        Minus_4 = -9,

        [Description("-2")]
        Minus_2 = -8,

        [Description("-1")]
        Minus_1 = -7,

        [Description("-1/2")]
        Minus_1_2 = -6,

        [Description("-1/4")]
        Minus_1_4 = -5,

        [Description("-1/8")]
        Minus_1_8 = -4,

        [Description("-1/16")]
        Minus_1_16 = -3,

        [Description("-Fine")]
        Minus_Fine = -2,

        [Description("-UltraFine")]
        Minus_UltraFine = -1,

        None = 0,

        [Description("+UltraFine")]
        Plus_UltraFine = 1,

        [Description("+Fine")]
        Plus_Fine = 2,

        [Description("+1/16")]
        Plus_1_16 = 3,

        [Description("+1/8")]
        Plus_1_8 = 4,

        [Description("+1/4")]
        Plus_1_4 = 5,

        [Description("+1/2")]
        Plus_1_2 = 6,

        [Description("+1")]
        Plus_1 = 7,

        [Description("+2")]
        Plus_2 = 8,

        [Description("+4")]
        Plus_4 = 9,

        [Description("+8")]
        Plus_8 = 10,

        [Description("+16")]
        Plus_16 = 11,

        [Description("+32")]
        Plus_32 = 12,

        [Description("+Loop")]
        Plus_Loop = 13
    }
}
