
namespace cmdr.MidiLib.Devices
{
    public abstract class MidiDevice
    {
        private readonly int _id;
        public int Id { get { return _id; } }

        private readonly string _name;
        public string Name { get { return _name; } }


        internal MidiDevice(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public abstract void Reset();
    }
}
