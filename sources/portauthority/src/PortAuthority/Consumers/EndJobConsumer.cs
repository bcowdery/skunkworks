using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;

namespace PortAuthority.Consumers
{
    public class EndJobConsumer
        : IConsumer<EndJob>
    {
        private readonly ILogger<EndJobConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;

        public EndJobConsumer(ILogger<EndJobConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<EndJob> context)
        {
            var message = context.Message;

            _logger.LogInformation("Ending Job Id = {JobId}", message.JobId);

            var job = await _dbContext.Jobs.SingleOrDefaultAsync(x => x.JobId == message.JobId);
            if (job == null)
            {
                _logger.LogWarning("Job does not exist with Id = {JobId}", message.JobId);
                return;
            }

            if (job.IsFinished())
            {
                _logger.LogWarning("Job has already been ended. Id = {JobId}", message.JobId);
                return;                
            }
            
            job.Status = message.Success ? Status.Completed : Status.Failed;
            job.EndTime = message.EndTime;

            await _dbContext.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Endpoint configuration for the <see cref="EndJobConsumerDefinition"/>
    /// </summary>
    public class EndJobConsumerDefinition 
        : ConsumerDefinition<EndJobConsumer>
    {
        public EndJobConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = "port-authority-jobs";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<EndJobConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }            
}
