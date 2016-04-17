using System;

namespace cmdr.TsiLib.Ranges
{
    public class IntRange : ARange<Int32>
    {
        public IntRange(int minValue, int maxValue)
            : base(minValue, maxValue)
        {

        }
    }
}
