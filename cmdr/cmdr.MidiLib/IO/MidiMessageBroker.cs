using System;
using System.Collections;
using System.Threading.Tasks;
using cmdr.MidiLib.Channels;
using cmdr.MidiLib.Enums;
using cmdr.MidiLib.Messages;
using cmdr.MidiLib.Devices;

namespace cmdr.MidiLib.IO
{
    public class MidiMessageBroker
    {
        private readonly MidiInputDevice _sender;
        private cmdr.MidiLib.Core.MidiIO.InputDevice _device;
        private bool _running = false;

        private readonly BitArray _channelListeners;
        internal BitArray ChannelListeners { get { return _channelListeners; } }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        internal MidiMessageBroker(MidiInputDevice sender, BitArray channelListeners)
        {
            _sender = sender;
            _channelListeners = channelListeners;
        }


        public void Start()
        {
            if (_running)
                return;

            _running = true;
            try
            {
                _device = new cmdr.MidiLib.Core.MidiIO.InputDevice(_sender.Id);
                _device.OnMidiEvent += onMidiEventHandle;
                _device.Start();
            }
            catch (Exception)
            {
                _running = false;
            }
        }

        public void Stop()
        {
            if (!_running)
                return;

            try
            {
                _device.OnMidiEvent -= onMidiEventHandle;
                _device.Stop();
                Task.Factory.StartNew(new Action(_device.Dispose));
            }
            catch (Exception)
            {

            }
            _running = false;
        }


        private void onMidiEventHandle(cmdr.MidiLib.Core.MidiIO.Data.MidiEvent ev)
        {
            bool checkSysEx = false;
            
            var handleEvent = (ev.MidiEventType == cmdr.MidiLib.Core.MidiIO.Definitions.EMidiEventType.Short)
                ? _channelListeners[ev.Status & 0x0F]
                : checkSysEx;
            if (!handleEvent) 
                return;

            MidiChannel channel = MidiManager.Channels[(ev.Status & 0x0F) + 1];

            MidiMessage message = null;
            var type = getMidiEventType(ev);
            switch (type)
            {
                case MidiMessageType.ControlChange:
                    message = new MidiControlChangeMessage(ev, type);
                    break;
                case MidiMessageType.NoteOff:
                case MidiMessageType.NoteOn:
                    message = new MidiNoteMessage(ev, type);
                    break;
                case MidiMessageType.ProgramChange:
                    message = new MidiProgramChangeMessage(ev, type);
                    break;
                case MidiMessageType.PitchBend:
                    message = new MidiPitchBendMessage(ev, type);
                    break;
                default:
                    throw new Exception("MidiMessageType is unknown");
            }

            if (message != null && MessageReceived != null)
                MessageReceived(_sender, new MessageReceivedEventArgs(channel, message));
        }

        private MidiMessageType getMidiEventType(cmdr.MidiLib.Core.MidiIO.Data.MidiEvent ev)
        {
            MidiMessageType eventType;
            var shortType = ev.Status & 0xF0;
            switch (shortType)
            {
                case 0x80:
                    //Note Off
                    eventType = MidiMessageType.NoteOff;
                    break;
                case 0x90:
                    // Note On
                    eventType = MidiMessageType.NoteOn;
                    break;
                case 0xA0:
                    // Polyphonic Aftertouch
                    eventType = MidiMessageType.PolyAftertouch;
                    break;
                case 0xB0:
                    // Control Message
                    eventType = MidiMessageType.ControlChange;
                    break;
                case 0xC0:
                    // Program Change to general Midi Instrument Name
                    eventType = MidiMessageType.ProgramChange;
                    break;
                case 0xD0:
                    // Channel Aftertouch
                    eventType = MidiMessageType.ChannelAftertouch;
                    break;
                case 0xE0:
                    // Pitch Wheel / Pitch Bend
                    eventType = MidiMessageType.PitchBend;
                    break;
                default:
                    eventType = MidiMessageType.Unknown;
                    break;
            }

            return eventType;
        }


        public class MessageReceivedEventArgs : EventArgs
        {
            private readonly MidiChannel _channel;
            public MidiChannel Channel { get { return _channel; } }
            
            private readonly MidiMessage _message;
            public MidiMessage Message { get { return _message; } }


            internal MessageReceivedEventArgs(MidiChannel channel, MidiMessage message)
            {
                _channel = channel;
                _message = message;
            }
        }
    }
}
