using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public class TaskItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Reminder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ReminderDisplay => Reminder?.ToString("g") ?? "None";
    }
}
