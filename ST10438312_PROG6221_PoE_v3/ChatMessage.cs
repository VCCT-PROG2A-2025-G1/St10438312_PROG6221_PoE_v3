using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage(string message, bool isUser)
        {
            Message = message;
            IsUser = isUser;
            Timestamp = DateTime.Now;
        }
    }
}

