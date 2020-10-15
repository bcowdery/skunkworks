using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class CreateSubtaskConsumer
        : IConsumer<CreateSubtask>
    {
        private readonly ILogger<CreateSubtaskConsumer> _logger;

        public CreateSubtaskConsumer(ILogger<CreateSubtaskConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CreateSubtask> context)
        {
            throw new NotImplementedException();
        }
    }
}
