using System.ComponentModel;

namespace cmdr.TsiLib.Enums
{
    public enum LoopRecorderState
    {
        Empty = 0,
        Paused = 1,
        Playing = 2,
        Record = 3,
        [Description("Record (Overdub Mode)")]
        RecordOverdubMode = 4
    }
}
