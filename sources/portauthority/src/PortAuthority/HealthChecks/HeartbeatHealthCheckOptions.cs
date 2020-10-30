using System;

namespace PortAuthority.HealthChecks
{
    /// <summary>
    /// Heartbeat health check options
    /// </summary>
    public class HeartbeatHealthCheckOptions
    {
        /// <summary>
        /// Max time to wait for a checkin before marking an endpoint as degraded
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        
        /// <summary>
        /// Max age of degraded endpoints before removing the heartbeat
        /// </summary>
        public TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(5);
    }
}
