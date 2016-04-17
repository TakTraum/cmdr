using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal abstract class Frame
    {
        private const int FRAME_ID_FIXED_LENGTH = 4;

        public string FrameId { get; protected set; }

        protected int? FrameSizeOnDisk { get; private set; }

        public abstract void Write(Writer writer);


        protected Frame(Stream stream)
        {
            FrameId = stream.ReadASCIIString(4);
            FrameSizeOnDisk = stream.ReadInt32BigE();
        }

        protected Frame(string id)
        {
            if (String.IsNullOrEmpty(id) || id.Length != FRAME_ID_FIXED_LENGTH)
                throw new ArgumentException("id must be 4 characters exactly");
            FrameId = id;
        }


        public static string PeekFourCC(Stream stream)
        {
            var nextFourCC = stream.ReadASCIIString(4);
            stream.Position -= 4;
            return nextFourCC;
        }
    }
}