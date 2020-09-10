using System.Collections.Generic;

namespace CC.Models
{
    public class LogObject
    {
        public string EventName { get; set; }
        public string UserId { get; set; }

        public Dictionary<string, object> EventDetails { get; set; }
    }
}
