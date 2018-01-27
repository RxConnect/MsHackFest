using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxConnectSite.Models
{
    public class DoorMessage
    {
        public long Id { get; set; }
        public string State { get; set; }
        public string DoorId { get; set; }
        public DateTime Enqueued { get; set; }
    }
}
