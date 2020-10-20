using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Consumers
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
    
    /// <summary>
    /// Endpoint configuration for the <see cref="StartSubtaskConsumerDefinition"/>
    /// </summary>
    public class StartSubtaskConsumerDefinition
        : ConsumerDefinition<StartSubtaskConsumer>
    {
        public StartSubtaskConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = PortAuthorityEndpointConventions.SubtaskEndpoint;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<StartSubtaskConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }        
}
