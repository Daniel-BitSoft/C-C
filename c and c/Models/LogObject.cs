using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models
{
    public class LogObject
    {
        public string EventName { get; set; }
        public string UserId { get; set; }

        public Dictionary<string, object> EventDetails { get; set; }
    }
}
