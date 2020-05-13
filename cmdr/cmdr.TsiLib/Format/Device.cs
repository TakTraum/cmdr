using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class Device : Frame
    {
        public string DeviceType { get; set; }
        public DeviceData Data { get; private set; }

        private bool _isKeyboard;
        public bool IsKeyboard
        {
            get
            {
                return _isKeyboard;
            }
            set
            {
                _isKeyboard = value;
            }
        }

        public Device(string deviceType, string traktorVersion, bool RemoveUnusedMIDIDefinition, bool isKeyboard)
            : base("DEVI")
        {
            DeviceType = deviceType;
            IsKeyboard = isKeyboard;
            Data = new DeviceData(traktorVersion);
        }

        public Device(Stream stream)
            : base(stream)
        {
            DeviceType = stream.ReadWideStringBigE();
            Data = new DeviceData(stream);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteWideStringBigE(DeviceType);
            Data.Write(writer);

            writer.EndFrame();
        }
    }
}
