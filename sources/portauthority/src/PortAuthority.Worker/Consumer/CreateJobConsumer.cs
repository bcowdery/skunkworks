using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class CreateJobConsumer
        : IConsumer<CreateJob>
    {
        private readonly ILogger<CreateJobConsumer> _logger;

        public CreateJobConsumer(ILogger<CreateJobConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CreateJob> context)
        {
            throw new NotImplementedException();
        }
    }
}