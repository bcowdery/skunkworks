using System;
using System.Collections.Generic;

namespace PortAuthority.HealthChecks
{
    /// <summary>
    /// Monitors heartbeats of worker services
    /// </summary>
    public interface IHeartbeatMonitor
    {
        bool CheckHeartbeats(TimeSpan threshold, TimeSpan maxAge, out List<HeartbeatStatus> heartbeats);
        void AddHeartbeat(Uri sourceAddress, DateTimeOffset heartbeat);
        void RemoveHeartbeat(Uri sourceAddress);
    }
}
