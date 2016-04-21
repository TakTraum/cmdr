using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib.Parsers
{
    public class EffectParser : IParser<Effect>
    {
        public static List<Effect> AllValues;

        static EffectParser()
        {
            // Effects strongly depend on TraktorSettings. Not the Enum value is stored in TSI files, 
            // but the index of the chosen effect in TraktorSettings' "pre-selected" list of effects.
            // TraktorSettings must be initialized first.
            try
            {
                AllValues = new List<Effect>(TraktorSettings.Instance.Effects);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                AllValues = new List<Effect> { Effect.NoEffect };
            }
        }

        public Effect DecodeValue(byte[] rawValue)
        {
            int fxIdx = BitConverter.ToInt32(rawValue, 0);
            if (fxIdx == 0)
                return Effect.NoEffect;
            else
                fxIdx -= 1;

            if (fxIdx < AllValues.Count)
                return AllValues[fxIdx];
            else
                return Effect.NoEffect;
        }

        public byte[] EncodeValue(Effect value)
        {
            int fxIdx = AllValues.IndexOf(value);
            if (fxIdx < 0)
                fxIdx = 0;
            else
                fxIdx += 1;
            return BitConverter.GetBytes(fxIdx);
        }
    }
}
