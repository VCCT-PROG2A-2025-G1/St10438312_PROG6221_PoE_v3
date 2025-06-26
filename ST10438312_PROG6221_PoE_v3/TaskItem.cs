using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    public class TaskItem
    {
        // Task model class
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Reminder { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime? CompletedDate { get; set; }

            public string ReminderDate => Reminder.ToString("d");
            public string ReminderTime => Reminder.ToString("t");
            public string TimeRemaining
            {
                get
                {
                    TimeSpan remaining = Reminder - DateTime.Now;
                    if (remaining.TotalDays >= 1)
                        return $"Due in {Math.Floor(remaining.TotalDays)} days";
                    if (remaining.TotalHours >= 1)
                        return $"Due in {Math.Floor(remaining.TotalHours)} hours";
                    return "Due soon";
                }
            }
        }
    }

