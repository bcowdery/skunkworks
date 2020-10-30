using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PortAuthority.HealthChecks
{
    /// <summary>
    /// Health check that reports missed heartbeats from worker services
    /// </summary>
    public class HeartbeatHealthCheck
        : IHealthCheck
    {
        private readonly ILogger<HeartbeatHealthCheck> _logger;
        private readonly IOptions<HeartbeatHealthCheckOptions> _heartbeatOptions;
        private readonly IHeartbeatMonitor _heartbeatMonitor;

        public HeartbeatHealthCheck(
            ILogger<HeartbeatHealthCheck> logger,
            IOptions<HeartbeatHealthCheckOptions> heartbeatOptions,
            IHeartbeatMonitor heartbeatMonitor)
        {
            _logger = logger;
            _heartbeatOptions = heartbeatOptions;
            _heartbeatMonitor = heartbeatMonitor;
        }

        /// <summary>
        /// Checks the heartbeat monitor and reports any workers that did not have callback within the last 30s
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var options = _heartbeatOptions.Value;
            var isHealthy = _heartbeatMonitor.CheckHeartbeats(options.Timeout, options.MaxAge, out var heartbeats);
            
            var data = new Dictionary<string, object>() { ["Workers"] = heartbeats };

            var result = isHealthy
                ? HealthCheckResult.Healthy("Ready", data)
                : heartbeats.Any()
                    ? HealthCheckResult.Degraded($"Worker did not report heartbeat within {options.Timeout}", data: data)
                    : HealthCheckResult.Unhealthy($"No workers running", data: data);

            if (!isHealthy)
            {
                _logger.LogWarning("Workers are degraded and did not report a heartbeat within {Threshold}", options.Timeout);    
            }
            
            return Task.FromResult(result);
        }
    }
}
