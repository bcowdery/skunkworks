using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Events;

namespace PortAuthority.Worker.Bootstrap
{
    public class HeartbeatBackgroundService
        : BackgroundService
    {
        private readonly ILogger<HeartbeatBackgroundService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public HeartbeatBackgroundService(ILogger<HeartbeatBackgroundService> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Ghetto timer for sending heartbeats every 10 seconds
            // This should be a recurring scheduled message or a Quartz.net scheduled job
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTimeOffset.UtcNow;
                
                _logger.LogDebug("Heartbeat as of {Now}", now);

                await _publishEndpoint.Publish<Heartbeat>(new {Timestamp = now}, stoppingToken);
                
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
