using System;
using System.Runtime.InteropServices;

namespace cmdr.MidiLib.Core.MidiIO.Data
{
	/// <summary>
	/// Builds a pointer to a MidiHeader structure.
	/// </summary>
	internal class MidiHeaderBuilder
	{
        // The length of the system exclusive buffer.
        private int _bufferLength;

        // The system exclusive data.
        private byte[] _data;

        // Indicates whether the pointer to the MidiHeader has been built.
        private bool _built;

        // The built pointer to the MidiHeader.
        private IntPtr _result;

        /// <summary>
        /// Initializes a new instance of the MidiHeaderBuilder.
        /// </summary>
		public MidiHeaderBuilder()
		{
            BufferLength = 1;
		}

        #region Methods

        /// <summary>
        /// Builds the pointer to the MidiHeader structure.
        /// </summary>
        public void Build()
        {
            var header = new MidiHeader
                                    {
                                        bufferLength = BufferLength,
                                        bytesRecorded = 0,
                                        data = Marshal.AllocHGlobal(BufferLength),
                                        flags = 0
                                    };

            // Initialize the MidiHeader.

            // Write data to the MidiHeader.
            for(var i = 0; i < BufferLength; i++)
            {
                Marshal.WriteByte(header.data, i, _data[i]);
            }

            try
            {
                _result = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MidiHeader)));
            }
            catch(Exception)
            {
                Marshal.FreeHGlobal(header.data);

                throw;
            }

            try
            {
                Marshal.StructureToPtr(header, _result, false);
            }
            catch(Exception)
            {
                Marshal.FreeHGlobal(header.data);
                Marshal.FreeHGlobal(_result);

                throw;
            }

            _built = true;
        }

        /// <summary>
        /// Initializes the MidiHeaderBuilder with the specified SysExMessage.
        /// </summary>
        /// <param name="message">
        /// The SysExMessage to use for initializing the MidiHeaderBuilder.
        /// </param>
        public void InitializeBuffer(byte[] message)
        {
        //    // If this is a start system exclusive message.
        //    if(message.SysExType == SysExType.Start)
        //    {
                BufferLength = message.Length;

                // Copy entire message.
                for(int i = 0; i < BufferLength; i++)
                {
                    _data[i] = message[i];
                }
        //    }
        //    // Else this is a continuation message.
        //    else
        //    {
        //        BufferLength = message.Length - 1;

        //        // Copy all but the first byte of message.
        //        for(int i = 0; i < BufferLength; i++)
        //        {
        //            data[i] = message[i + 1];
        //        }
        //    }
        }

        //public void InitializeBuffer(ICollection events)
        //{
        //    #region Require

        //    if(events == null)
        //    {
        //        throw new ArgumentNullException("events");
        //    }
        //    else if((events.Count) % 4 != 0)
        //    {
        //        throw new ArgumentException("Stream events not word aligned.");
        //    }

        //    #endregion

        //    #region Guard

        //    if(events.Count == 0)
        //    {
        //        return;
        //    }

        //    #endregion

        //    BufferLength = events.Count;

        //    events.CopyTo(data, 0);
        //}

        /// <summary>
        /// Releases the resources associated with the built MidiHeader pointer.
        /// </summary>
        public void Destroy()
        {
            #region Require

            if(!_built)
            {
                throw new InvalidOperationException("Cannot destroy MidiHeader");
            }

            #endregion

            Destroy(_result);
        }

        /// <summary>
        /// Releases the resources associated with the specified MidiHeader pointer.
        /// </summary>
        /// <param name="headerPtr">
        /// The MidiHeader pointer.
        /// </param>
        public void Destroy(IntPtr headerPtr)
        {
            var header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

            Marshal.FreeHGlobal(header.data);
            Marshal.FreeHGlobal(headerPtr);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The length of the system exclusive buffer.
        /// </summary>
        public int BufferLength
        {
            get
            {
                return _bufferLength;
            }
            set
            {
                #region Require

                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("BufferLength", value, 
                        "MIDI header buffer length out of range.");
                }

                #endregion

                _bufferLength = value;
                _data = new byte[value];
            }
        }

        /// <summary>
        /// Gets the pointer to the MidiHeader.
        /// </summary>
        public IntPtr Result
        {
            get
            {
                return _result;
            }
        }

        #endregion
	}
}
