using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxConnectSite.Models
{
    public class Notifications
    {
        [Key]
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public int FobNumber { get; set; }
    }
}
