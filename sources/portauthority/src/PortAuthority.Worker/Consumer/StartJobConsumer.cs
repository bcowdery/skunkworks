using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Worker.Consumer
{
    public class StartJobConsumer 
        : IConsumer<StartJob>
    {
        private readonly ILogger<StartJobConsumer> _logger;

        public StartJobConsumer(ILogger<StartJobConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<StartJob> context)
        {
            throw new NotImplementedException();
        }
    }
}
