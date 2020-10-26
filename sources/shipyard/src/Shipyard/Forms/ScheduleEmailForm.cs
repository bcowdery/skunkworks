using System;
using NodaTime;
using Shipyard.Models;

namespace Shipyard.Forms
{
    public class ScheduledEmailForm
    {
        public EmailModel Email { get; set; }
        
        public DateTimeZone TimeZone { get; set; }
        public DateTimeOffset? ScheduleDate { get; set; }
    }
}
