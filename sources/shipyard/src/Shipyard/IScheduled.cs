using System;
using NodaTime;

namespace Shipyard
{
    public class IScheduled
    {
        public DateTimeZone? TimeZone { get; set; }
        public OffsetDateTime? ScheduleDate { get; set; }
    }
}
