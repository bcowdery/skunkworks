using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class StartSubtaskConsumer
        : IConsumer<StartSubtask>
    {
        private readonly ILogger<StartSubtask> _logger;

        public StartSubtaskConsumer(ILogger<StartSubtask> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<StartSubtask> context)
        {
            throw new NotImplementedException();
        }
    }
}
