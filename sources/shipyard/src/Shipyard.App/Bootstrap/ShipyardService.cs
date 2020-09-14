using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shipyard.App.Bootstrap
{
    public class ShipyardService : IHostedService
    {
        private readonly ILogger<ShipyardService> _logger;

        public ShipyardService(ILogger<ShipyardService> logger)
        {
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shipyard service starting ...");

            return Task.FromResult(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shipyard service stopping ...");

            return Task.FromResult(0);
        }
    }
}
