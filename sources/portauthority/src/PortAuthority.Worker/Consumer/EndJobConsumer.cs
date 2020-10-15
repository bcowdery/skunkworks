using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class EndJobConsumer
        : IConsumer<EndJob>
    {
        private readonly ILogger<EndJobConsumer> _logger;

        public EndJobConsumer(ILogger<EndJobConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<EndJob> context)
        {
            throw new NotImplementedException();
        }
    }
}