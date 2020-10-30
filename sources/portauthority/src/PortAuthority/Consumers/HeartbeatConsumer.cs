using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts;
using PortAuthority.Contracts.Events;
using PortAuthority.HealthChecks;

namespace PortAuthority.Consumers
{
    /// <summary>
    /// Collects heartbeat events from workers and updates the heartbeat monitor.
    /// </summary>
    public class HeartbeatConsumer
        : IConsumer<Heartbeat>
    {
        private readonly ILogger<HeartbeatConsumer> _logger;
        private readonly IHeartbeatMonitor _heartbeatMonitor;

        public HeartbeatConsumer(ILogger<HeartbeatConsumer> logger, IHeartbeatMonitor heartbeatMonitor)
        {
            _logger = logger;
            _heartbeatMonitor = heartbeatMonitor;
        }

        public Task Consume(ConsumeContext<Heartbeat> context)
        {
            _logger.LogDebug("Heartbeat source = {SourceAddress}, timestamp = {Timestamp}", context.SourceAddress, context.Message.Timestamp);

            _heartbeatMonitor.AddHeartbeat(context.SourceAddress, context.Message.Timestamp);

            return Task.CompletedTask;
        }
    }

    public class HeartbeatConsumerDefinition
        : ConsumerDefinition<HeartbeatConsumer>
    {
        public HeartbeatConsumerDefinition()
        {
            EndpointName = PortAuthorityEndpointConventions.HeartbeatExchange;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<HeartbeatConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            /*endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));*/

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }
}
