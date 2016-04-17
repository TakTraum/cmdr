using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cmdr.TsiLib.Utils
{
    internal class Writer
    {
        private class FrameTracker
        {
            public readonly int HeaderSize = 2*4; // 4 bytes header + 4 bytes size

            /// <summary>
            /// The position in the stream where the size of the frame should be written to
            /// </summary>
            public long SizeOffsetInStream { get; set; }
            public int Size { get; set; }
        }

        private readonly Stack<FrameTracker> _frames = new Stack<FrameTracker>();
        private readonly Stream _stream;
        
        public Writer(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream is null.");
            _stream = stream;
        }

        public void BeginFrame(string id)
        {
            FrameTracker tracker = new FrameTracker();
            _frames.Push(tracker);
            writeAsciiBigE(id, incrementSize: false);
            tracker.SizeOffsetInStream = _stream.Position;
            writeBigE(0, incrementSize: false); // Size placeholder
        }

        public void EndFrame()
        {
            FrameTracker tracker = _frames.Pop();

            long currentPosition = _stream.Position;
            _stream.Seek(tracker.SizeOffsetInStream, SeekOrigin.Begin);
            writeBigE(tracker.Size, incrementSize: false);
            _stream.Seek(currentPosition, SeekOrigin.Begin);

            if (_frames.Any())
                _frames.Peek().Size += tracker.Size + tracker.HeaderSize; // parent frame size increase
        }

        #region Write Utils

        public void WriteBigE(byte[] bytes)
        {
            writeBigE(bytes, true, false, true);
        }

        public void WriteBigE(int value)
        {
            writeBigE(value, incrementSize: true);
        }

        public void WriteBigE(bool value)
        {
            WriteBigE(value ? 1 : 0);
        }

        public void WriteBigE(float value)
        {
            byte[]bytes = BitConverter.GetBytes(value);
            writeBigE(bytes, true, false, false);
        }

        public void WriteAsciiBigE(string value)
        {
            writeAsciiBigE(value, true);
        }

        public void WriteWideStringBigE(string value)
        {
            int length = (value == null) ? 0 : value.Length;
            WriteBigE(length);
            writeBigE(Encoding.BigEndianUnicode.GetBytes(value), true, true, false);
        }

        public void WriteList<T>(IEnumerable<T> list) where T : Format.Frame
        {
            WriteBigE(list.Count());
            foreach (var item in list)
                item.Write(this);

        }


        private void writeBigE(byte[] bytes, bool incrementSize, bool writeAsIs, bool restoreEndianness)
        {
            if (!writeAsIs && BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            _stream.Write(bytes, 0, bytes.Length);

            if (incrementSize)
                _frames.Peek().Size += bytes.Length;

            if (!writeAsIs && restoreEndianness && BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
        }

        private void writeBigE(int value, bool incrementSize)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            writeBigE(bytes, incrementSize, false, false);
        }

        private void writeAsciiBigE(string value, bool incrementSize)
        {
            // Encoding.ASCII.GetBytes(string) returns byte array as big endian! 
            writeBigE(Encoding.ASCII.GetBytes(value), incrementSize, true, false);
        }

        #endregion
    }
}
