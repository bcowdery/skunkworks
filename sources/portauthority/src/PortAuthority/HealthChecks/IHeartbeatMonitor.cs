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
    
    /// <summary>
    /// Heartbeat status
    /// </summary>
    public readonly struct HeartbeatStatus
    {
        public HeartbeatStatus(Uri sourceAddress, TimeSpan age, bool isAlive)
        {
            SourceAddress = sourceAddress;
            Age = age;
            IsAlive = isAlive;
        }

        /// <summary>
        /// Source address of the heartbeat
        /// </summary>
        public Uri SourceAddress { get; }
        
        /// <summary>
        /// Age of the entry, how long ago was the last heartbeat recorded.
        /// </summary>
        public TimeSpan Age { get; }
        
        /// <summary>
        /// True if endpoint is alive, false if heartbeat is outside of the configured threshold
        /// </summary>
        public bool IsAlive { get; }
    }    
}
