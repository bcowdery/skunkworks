using System;
using Shipyard.Contracts.MessageTypes;

namespace Shipyard.Contracts.Commands 
{
    public interface ScheduleEmail
    {
        IEmail Email { get; }
        DateTimeOffset? ScheduleTime { get; }
    }
}
