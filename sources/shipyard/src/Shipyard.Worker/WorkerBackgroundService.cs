using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shipyard.Worker
{
    public class WorkerBackgroundService : BackgroundService
    {
        private readonly ILogger<WorkerBackgroundService> _logger;

        public WorkerBackgroundService(ILogger<WorkerBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // proof of concept
            // print a status message every 10 seconds
            
            _logger.LogDebug("Hello there!");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
