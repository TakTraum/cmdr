
namespace cmdr.Editor.ViewModels.MidiBinding
{
    public class MidiSignal
    {
        public int Channel { get; private set; }
        public string Note { get; private set; }

        public MidiSignal(int channel, string note)
        {
            Channel = channel;
            Note = note;
        }
    }
}
