
namespace cmdr.TsiLib.Enums
{
    public enum PlaybackMode
    {
        /*
        InternalMode = 0,
        RelativeMode = 1,
        AbsoluteMode = 2,
        HapticMode = 3,
        */

        // per traktor 3.4, this is the new values. Needs testing to see if its a bug
        // see tests/favorites_enum.tsi
        AbsoluteMode = 0,
        RelativeMode = 1,
        InternalMode = 2,
        HapticMode = 3,
        

    }
}
