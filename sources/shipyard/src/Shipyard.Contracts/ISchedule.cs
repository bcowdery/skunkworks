using System;
using NodaTime;

namespace Shipyard
{
    public class ISchedule
    {
        public DateTimeZone TimeZone { get; }
        public OffsetDateTime? ScheduleDate { get; }
    }
}
