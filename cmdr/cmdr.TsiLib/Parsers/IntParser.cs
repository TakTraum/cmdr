using System;

namespace cmdr.TsiLib.Parsers
{
    public class IntParser : IParser<Int32>
    {
        public int DecodeValue(byte[] rawValue)
        {
            return BitConverter.ToInt32(rawValue, 0);
        }

        public byte[] EncodeValue(int value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
