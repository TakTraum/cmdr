
namespace cmdr.TsiLib
{
    public static class EffectList
    {
        private static string[] _all = new string[] { };
        public static string[] All
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return _all;
            }
        }

        private static bool _initialized = false;

        private static void Initialize()
        {
            _all = new string[]{ 
                                "Delay",
                                "Reverb",
                                "Flanger"
                                };
            _initialized = true;
        }
    }
}
