using System;
using System.ComponentModel.DataAnnotations;

namespace RxConnectSite.Models
{
    public class Fobs
    {
        [Key]
        public Guid         FobId         { get; set; }   // Device Id
        public int          FobNumber     { get; set; }
        public string       DeviceNumber  { get; set; }
        public DateTime?    LastAccess    { get; set; }
        public int          LastChannel   { get; set; }
        public int          LastCounter   { get; set; }
        public bool         Locked        { get; set; }
        public CommandType  Command       { get; set; }
    }

    public enum CommandType
    {
        Undefined       = 0,
        Activation      = 1,
        Prog            = 2,
        Radio           = 3,
        Substitution    = 4,
        Pin             = 5,
        Erase           = 6,
        MemoryErase     = 7,
        Impersonation   = 8,
    }
}
