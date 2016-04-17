using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    internal static class StreamExtensions
    {
        #region Read

        public static byte[] ReadBytesBigE(this Stream stream, int length)
        {
            byte[]bytes = new byte[length];
            stream.Read(bytes, 0, length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(this Stream stream)
        {
            long oldPos = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = ReadBytesBigE(stream, (int)stream.Length);
            stream.Seek(oldPos, SeekOrigin.Begin);
            return bytes;
        }

        public static string ReadASCIIString(this Stream stream, int length)
        {
            byte[] bytes = stream.ReadBytesBigE(length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return Encoding.ASCII.GetString(bytes);
        }

        public static float ReadFloatBigE(this Stream stream)
        {
            byte[]bytes = stream.ReadBytesBigE(4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static int ReadInt32BigE(this Stream stream)
        {
            byte[]bytes = stream.ReadBytesBigE(4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static string ReadWideStringBigE(this Stream stream)
        {
            int length = stream.ReadInt32BigE() * 2;
            byte[] bytes = stream.ReadBytesBigE(length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return Encoding.BigEndianUnicode.GetString(bytes);
        }

        public static bool ReadBoolBigE(this Stream stream)
        {
            return stream.ReadInt32BigE() == 1;
        }

        public static List<T> ReadList<T>(this Stream stream) where T : cmdr.TsiLib.Format.Frame
        {
            int count = stream.ReadInt32BigE();
            var list = new List<T>();

            for (int i = 0; i < count; i++)
                list.Add((T)Activator.CreateInstance(typeof(T), stream));
            return list;
        }

        #endregion

    }
}
