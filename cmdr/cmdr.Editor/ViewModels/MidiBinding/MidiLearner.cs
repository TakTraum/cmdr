using cmdr.MidiLib;
using cmdr.MidiLib.Enums;
using cmdr.MidiLib.IO;
using cmdr.MidiLib.Messages;
using cmdr.MidiLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.Editor.ViewModels.MidiBinding
{
    public class MidiLearner
    {
        private readonly Action<MidiSignal> onMessageCallback;
        private readonly KeyConverter _keyConverter = new KeyConverter();

        private List<MidiMessageBroker> _brokers;

        private volatile bool _isActive;
        private volatile MidiSignal _lastSignal;


        public MidiLearner(Action<MidiSignal> callback)
        {
            onMessageCallback = callback;
            _brokers = new List<MidiMessageBroker>();
        }


        public bool CanLearn()
        {
            var inPorts = MidiManager.InputDevices;
            _brokers = inPorts.Select(d => d.GetMessageBroker()).ToList();
            return _brokers.Any();
        }

        public bool CanLearn(string inPort)
        {
            var inPorts = MidiManager.InputDevices.Where(d => d.Name.Equals(inPort));
            _brokers = inPorts.Select(d => d.GetMessageBroker()).ToList();
            return _brokers.Any();
        }

        public bool Start()
        {
            foreach (var broker in _brokers)
            {
                try
                {
                    broker.MessageReceived += onMidiMessageReceived;
                    broker.Start();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    broker.MessageReceived -= onMidiMessageReceived;
                    return false;
                }
            }
            
            _isActive = true;
            
            return true;
        }


        public void Stop()
        {
            _isActive = false;

            foreach (var broker in _brokers)
            {
                try
                {
                    broker.MessageReceived -= onMidiMessageReceived;
                    broker.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }




        void onMidiMessageReceived(object sender, MidiMessageBroker.MessageReceivedEventArgs e)
        {
            if (!_isActive)
                return;

            int channel = e.Channel.Number;
            string note = String.Empty;

            switch (e.Message.Type)
            {
                case MidiMessageType.ControlChange:
                    MidiControlChangeMessage ccMsg = e.Message as MidiControlChangeMessage;
                    note = String.Format("CC.{0:000}", ccMsg.ControlId);
                    break;
                case MidiMessageType.NoteOff:
                case MidiMessageType.NoteOn:
                    MidiNoteMessage noteMsg = e.Message as MidiNoteMessage;
                    note = "Note." + _keyConverter.GetKeyTextIPN(noteMsg.Key);
                    break;
                case MidiMessageType.PitchBend:
                    note = "PitchBend";
                    break;
                default:
                    return;
            }

            _lastSignal = new MidiSignal(channel, note);
            
            notify();
        }

        private void notify()
        {
            onMessageCallback(_lastSignal);
        }

    }
}
