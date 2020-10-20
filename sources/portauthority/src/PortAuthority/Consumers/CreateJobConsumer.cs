using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;

namespace PortAuthority.Consumers
{
    /// <summary>
    /// Consumer that creates a Job.
    /// </summary>
    public class CreateJobConsumer
        : IConsumer<CreateJob>
    {
        private readonly ILogger<CreateJobConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;
        
        public CreateJobConsumer(ILogger<CreateJobConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<CreateJob> context)
        {
            var message = context.Message;
            if (message.JobId == Guid.Empty)
            {
                _logger.LogWarning("Job Id cannot be empty.");
                return;
            }
            
            _logger.LogInformation("Creating Job Id = {JobId}, Type = {Type}, Namespace = {Namespace}", 
                message.JobId, 
                message.Type, 
                message.Namespace);
            
            var job = new Job()
            {
                JobId = message.JobId,
                CorrelationId = message.CorrelationId,
                Type = message.Type,
                Namespace = message.Namespace,
            };

            await _dbContext.Jobs.AddAsync(job);
            await _dbContext.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Endpoint configuration for the <see cref="CreateJobConsumer"/>
    /// </summary>
    public class CreateJobConsumerDefinition 
        : ConsumerDefinition<CreateJobConsumer>
    {
        public CreateJobConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = PortAuthorityEndpointConventions.JobEndpoint;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CreateJobConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }    
}
