namespace cmdr.MidiLib.Core.MidiIO.Exceptions
{
    internal enum EDeviceException
    {
        MmsyserrAllocated = 4,
        MmsyserrBaddb = 14,
        MmsyserrBaddeviceid = 2,
        MmsyserrBaderrnum = 9,
        MmsyserrDeleteerror = 0x12,
        MmsyserrError = 1,
        MmsyserrHandlebusy = 12,
        MmsyserrInvalflag = 10,
        MmsyserrInvalhandle = 5,
        MmsyserrInvalidalias = 13,
        MmsyserrInvalparam = 11,
        MmsyserrKeynotfound = 15,
        MmsyserrLasterror = 20,
        MmsyserrNodriver = 6,
        MmsyserrNodrivercb = 20,
        MmsyserrNoerror = 0,
        MmsyserrNomem = 7,
        MmsyserrNotenabled = 3,
        MmsyserrNotsupported = 8,
        MmsyserrReaderror = 0x10,
        MmsyserrValnotfound = 0x13,
        MmsyserrWriteerror = 0x11,
        MidierrBadopenmode = 70,
        MidierrDontContinue = 0x47,
        MidierrInvalidsetup = 0x45,
        MidierrLasterror = 0x47,
        MidierrNodevice = 0x44,
        MidierrNomap = 0x42,
        MidierrNotready = 0x43,
        MidierrStillplaying = 0x41,
        MidierrUnprepared = 0x40
    }
}