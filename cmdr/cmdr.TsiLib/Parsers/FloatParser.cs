using System;

namespace cmdr.TsiLib.Parsers
{
    public class FloatParser : IParser<float>
    {
        public float DecodeValue(byte[] rawValue)
        {
            return BitConverter.ToSingle(rawValue, 0);
        }

        public byte[] EncodeValue(float value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
