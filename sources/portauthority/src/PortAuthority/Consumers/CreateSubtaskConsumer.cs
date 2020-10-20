using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Consumers
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

    /// <summary>
    /// Endpoint configuration for the <see cref="CreateSubtaskConsumer"/>
    /// </summary>
    public class CreateSubtaskConsumerDefinition
        : ConsumerDefinition<CreateSubtaskConsumer>
    {
        public CreateSubtaskConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = "port-authority-tasks";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<CreateSubtaskConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }
}
