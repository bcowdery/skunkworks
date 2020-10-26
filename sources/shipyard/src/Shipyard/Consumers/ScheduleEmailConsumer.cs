using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Shipyard.Contracts;
using Shipyard.Contracts.Commands;

namespace Shipyard.Consumers
{
    public class ScheduleEmailConsumer
        : IConsumer<ScheduleEmail>
    {
        public Task Consume(ConsumeContext<ScheduleEmail> context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ScheduleEmailConsumerDefinition
        : ConsumerDefinition<ScheduleEmailConsumer>
    {
        public ScheduleEmailConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = ShipyardEndpointConventions.ScheduleEndpoint;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        public void Configure(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ScheduleEmailConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }
}
