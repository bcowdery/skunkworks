using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;

namespace PortAuthority.Consumers
{
    /// <summary>
    /// Consumer that starts a sub-task and updates the status to <see cref="Status.InProgress"/>
    /// </summary>
    public class StartSubtaskConsumer
        : IConsumer<StartSubtask>
    {
        private readonly ILogger<StartSubtaskConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;

        public StartSubtaskConsumer(ILogger<StartSubtaskConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<StartSubtask> context)
        {
            var message = context.Message;

            _logger.LogInformation("Starting sub-task Id = {TaskId}", message.TaskId);

            var task = await _dbContext.Tasks.SingleOrDefaultAsync(x => x.TaskId == message.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Sub-task does not exist with Id = {TaskId}", message.TaskId);
                return;
            }

            if (!task.IsPending())
            {
                _logger.LogWarning("Sub-task has already been started. Id = {TaskId}", message.TaskId);
                return;                
            }
            
            task.Status = Status.InProgress;
            task.StartTime = message.StartTime;

            await _dbContext.SaveChangesAsync();
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
