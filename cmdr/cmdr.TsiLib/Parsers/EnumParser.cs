using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.TsiLib.Parsers
{
    public class EnumParser<T> : IParser<T> where T : struct, IConvertible
    {
        public static IEnumerable<T> AllValues = Enum.GetValues(typeof(T)).Cast<T>();


        public EnumParser()
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
        }


        public T DecodeValue(byte[] rawValue)
        {
            return getEnumValue(BitConverter.ToInt32(rawValue, 0));
        }

        public byte[] EncodeValue(T value)
        {
            return BitConverter.GetBytes(Convert.ToInt32(value));
        }


        private T getEnumValue(int intValue)
        {
            return (T)Enum.ToObject(typeof(T), intValue);
        }
    }
}
