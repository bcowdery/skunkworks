namespace Shipyard.Extensions
{
    public static class IScheduleExtensions
    {
        /// <summary>
        /// Returns true if the item can be scheduled.
        /// </summary>
        public static bool IsScheduled(this ISchedule scheduled) 
        {
            return scheduled.TimeZone != null || scheduled.ScheduleDate.HasValue;
        }

        /// <summary>
        /// Returns true if the scheduled item has an absolute schedule date.
        /// </summary>
        public static bool IsAbsolute(this ISchedule scheduled) 
        {
            return scheduled.ScheduleDate.HasValue;
        }

        /// <summary>
        /// Returns true if the scheduled item has a target time zone.
        /// </summary>
        public static bool IsZoned(this ISchedule scheduled) 
        {
            return scheduled.TimeZone != null;
        }
    }
}
