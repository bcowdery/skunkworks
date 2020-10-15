using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class EndSubtaskConsumer
        : IConsumer<EndSubtask>
    {
        private readonly ILogger<EndSubtask> _logger;

        public EndSubtaskConsumer(ILogger<EndSubtask> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<EndSubtask> context)
        {
            throw new NotImplementedException();
        }
    }
}
