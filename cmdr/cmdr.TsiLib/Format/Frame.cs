using cmdr.TsiLib.Utils;
using System;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal abstract class Frame
    {
        private const int FRAME_ID_FIXED_LENGTH = 4;

        protected int? FrameSizeOnDisk { get; private set; }

        public string FrameId { get; protected set; }


        protected Frame(Stream stream)
        {
            FrameId = stream.ReadASCIIString(FRAME_ID_FIXED_LENGTH);
            FrameSizeOnDisk = stream.ReadInt32BigE();
        }

        protected Frame(string id)
        {
            if (String.IsNullOrEmpty(id) || id.Length != FRAME_ID_FIXED_LENGTH)
                throw new ArgumentException("id must be " + FRAME_ID_FIXED_LENGTH + " characters exactly");
            FrameId = id;
        }


        public abstract void Write(Writer writer);


        public static string PeekFourCC(Stream stream)
        {
            var nextFourCC = stream.ReadASCIIString(FRAME_ID_FIXED_LENGTH);
            stream.Position -= FRAME_ID_FIXED_LENGTH;
            return nextFourCC;
        }
    }
}