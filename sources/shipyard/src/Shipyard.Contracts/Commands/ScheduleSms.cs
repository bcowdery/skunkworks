using System;
using Shipyard.Contracts.MessageTypes;

namespace Shipyard.Contracts.Commands 
{
    public interface ScheduleSms
    {
        ISms Sms { get; }
        DateTimeOffset? ScheduleTime { get; }
    }
}
