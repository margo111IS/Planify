using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planify.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public double DurationInHours { get; set; }
        public DateTime EndDate => StartDate.AddHours(DurationInHours);
        public string? Location { get; set; }
        public bool NeedsReminder { get; set; }

        public bool IsReminderMuted { get; set; } = false;

        public bool IsReminderDiscarded { get; set; } = false;
    }
}