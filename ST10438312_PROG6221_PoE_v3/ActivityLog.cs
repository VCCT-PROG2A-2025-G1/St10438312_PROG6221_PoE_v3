using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } // "Task", "Quiz", "Chat"
        public string Description { get; set; }
        public Dictionary<string, string> Details { get; set; } = new Dictionary<string, string>();
    }
}
