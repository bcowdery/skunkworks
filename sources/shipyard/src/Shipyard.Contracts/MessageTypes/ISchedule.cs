using System;
using NodaTime;

namespace Shipyard.Contracts.MessageTypes
{
    public interface ISchedule
    {
        public DateTimeZone TimeZone { get; }
    }
}
