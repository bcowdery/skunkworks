using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PortAuthority.Worker.Consumer
{
    /// <summary>
    /// Consumer that starts the job & updates the status to <see cref="Status.InProgress"/>.
    /// </summary>
    public class StartJobConsumer 
        : IConsumer<StartJob>
    {
        private readonly ILogger<StartJobConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;
        
        public StartJobConsumer(ILogger<StartJobConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<StartJob> context)
        {
            var message = context.Message;

            _logger.LogInformation("Starting Job Id = {JobId}", message.JobId);

            var job = await _dbContext.Jobs.SingleOrDefaultAsync(x => x.JobId == message.JobId);
            if (job == null)
            {
                _logger.LogWarning("Job does not exist with Id = {JobId}", message.JobId);
                return;
            }

            job.Status = Status.InProgress;
            job.StartTime = message.StartTime;

            await _dbContext.SaveChangesAsync();
        }
    }
    
    public class StartJobConsumerDefinition 
        : ConsumerDefinition<StartJobConsumer>
    {
        public StartJobConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = "port-authority";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<StartJobConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }        
}
