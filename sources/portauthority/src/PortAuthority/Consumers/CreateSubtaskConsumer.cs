using System;
using System.Collections.Generic;
using System.Text;
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
    /// Consumer that creates a sub-task for a job.
    /// </summary>
    public class CreateSubtaskConsumer
        : IConsumer<CreateSubtask>
    {
        private readonly ILogger<CreateSubtaskConsumer> _logger;
        private readonly IPortAuthorityDbContext _dbContext;

        public CreateSubtaskConsumer(ILogger<CreateSubtaskConsumer> logger, IPortAuthorityDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<CreateSubtask> context)
        {
            var message = context.Message;
            if (message.TaskId == Guid.Empty)
            {
                _logger.LogWarning("Sub-task id cannot be empty.");
                return;
            }
            
            _logger.LogInformation("Creating sub-task [JobId = {JobId}, TaskId = {JobId}, Name = {Name}]", 
                message.JobId,
                message.TaskId,
                message.Name);

            var job = await _dbContext
                .Jobs.AsNoTracking()
                .SingleOrDefaultAsync(x => x.JobId == message.JobId);
            
            if (job == null)
            {
                _logger.LogError("Job with ID {JobId} has not been created. Job creation may be queued and waiting. Retry.");
                throw new InvalidOperationException($"Job with ID {message.JobId} does not exist.");
            }
            
            var task = new Subtask()
            {
                JobId = job.Id,
                TaskId = message.TaskId,
                Name = message.Name,
            };

            await _dbContext.Tasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();
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
            EndpointName = PortAuthorityEndpointConventions.SubtaskEndpoint;
            
            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            /*ConcurrentMessageLimit = 8;*/
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<CreateSubtaskConsumer> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 5000));

            // use the outbox to prevent duplicate events from being published
            /*endpointConfigurator.UseInMemoryOutbox();*/
        }
    }
}
