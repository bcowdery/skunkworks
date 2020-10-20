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
    /// Consumer that updates a sub-task completion status on end time.
    /// </summary>    
    public class EndSubtaskConsumer
        : IConsumer<EndSubtask>
    {
        private readonly ILogger<EndSubtaskConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;

        public EndSubtaskConsumer(ILogger<EndSubtaskConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<EndSubtask> context)
        {
            var message = context.Message;

            _logger.LogInformation("Ending Sub-task Id = {TaskId}", message.TaskId);

            var task = await _dbContext.Tasks.SingleOrDefaultAsync(x => x.TaskId == message.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Sub-task does not exist with Id = {TaskId}", message.TaskId);
                return;
            }

            if (task.IsFinished())
            {
                _logger.LogWarning("Sub-task has already been ended. Id = {JobId}", message.TaskId);
                return;                
            }
            
            task.Status = message.Success ? Status.Completed : Status.Failed;
            task.EndTime = message.EndTime;

            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Endpoint configuration for the <see cref="EndSubtaskConsumerDefinition"/>
    /// </summary>
    public class EndSubtaskConsumerDefinition
        : ConsumerDefinition<EndSubtaskConsumer>
    {
        public EndSubtaskConsumerDefinition()
        {
            // override the default endpoint name
            EndpointName = PortAuthorityEndpointConventions.SubtaskEndpoint;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<EndSubtaskConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }    
}
