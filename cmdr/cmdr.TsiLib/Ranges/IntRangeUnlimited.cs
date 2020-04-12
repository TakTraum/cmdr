
namespace cmdr.TsiLib.Ranges
{
    public class IntRangeUnlimited : IntRange
    {
        public IntRangeUnlimited()
            : base(int.MinValue, int.MaxValue)
        {

        }
    }

    /*
     * ????
    public class IntRangeMIDILed: IntRange
    {
        public IntRangeUnlimited()
            : base(0, 127)
        {

        }
    }*/
}
