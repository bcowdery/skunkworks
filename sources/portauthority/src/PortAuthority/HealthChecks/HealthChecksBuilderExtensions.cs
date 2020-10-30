using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PortAuthority.HealthChecks
{
    /// <summary>
    /// Configuration extensions for custom health checks.
    /// </summary>
    public static class HealthChecksBuilderExtensions
    {
        private const string HEARTBEAT_NAME = "workers";

        private static readonly Action<HeartbeatHealthCheckOptions> DefaultHeartbeatConfig = options => { };
        
        
        /// <summary>
        /// Add a health check that reports worker heartbeats
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'worker_heartbeat' will be used for the name.</param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddWorkerHeartbeats(this IHealthChecksBuilder builder,
            Action<HeartbeatHealthCheckOptions> configure = null,
            string name = default, 
            IEnumerable<string> tags = default, 
            TimeSpan? timeout = default)
        {
            builder.Services.AddSingleton<HeartbeatHealthCheck>();
            builder.Services.AddSingleton<IHeartbeatMonitor, HeartbeatMonitor>();
            builder.Services.AddOptions<HeartbeatHealthCheckOptions>().Configure(configure ?? DefaultHeartbeatConfig);

            builder.Add(new HealthCheckRegistration(
                    name ?? HEARTBEAT_NAME,
                sp => sp.GetRequiredService<HeartbeatHealthCheck>(),
                    HealthStatus.Degraded,
                    tags,
                    timeout));

            return builder;
        }
    }
}
