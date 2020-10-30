using System;

namespace PortAuthority.Contracts.Events
{
    public interface Heartbeat
    {
        DateTimeOffset Timestamp { get; }
    }
}
