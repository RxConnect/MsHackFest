using System;
using System.ComponentModel.DataAnnotations;

namespace RxConnectSite.Models
{
    public class Subscribers
    {
        [Key]
        public Guid      DeviceId       { get; set; }   // Device Id
        public Guid      DistributorId  { get; set; }   // User Id
        public string    DeviceName     { get; set; }   // Device Name
        public string    SerialNumber   { get; set; }   // Serial Number
        public string    AccessKey      { get; set; }   // Access Key
        public string    OwnerEmail     { get; set; }   // Owner Mail
        public string    UnitVersion    { get; set; }   // Unit Version
        public int       Attempts       { get; set; }   // Attemps
        public bool      Locked         { get; set; }   // Locked
        public DateTime? Utc_Create     { get; set; }   // First Message
        public DateTime? Utc_LastPing   { get; set; }   // Last Message
    }
}
