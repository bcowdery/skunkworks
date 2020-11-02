using System;
using NodaTime;

namespace Shipyard.Contracts.MessageTypes
{
    public interface ISchedule
    {
        DateTimeZone TimeZone { get; }
    }
}
