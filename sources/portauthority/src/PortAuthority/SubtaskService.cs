using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortAuthority.Assemblers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;
using PortAuthority.Data.Queries;
using PortAuthority.Forms;
using PortAuthority.Models;
using PortAuthority.Results;

namespace PortAuthority
{
    public class SubtaskService
        : ISubtaskService
    {
        private readonly ILogger<SubtaskService> _logger;
        private readonly IAssembler<Subtask, SubtaskModel> _taskAssembler;
        private readonly IPortAuthorityDbContext _dbContext;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        
        public SubtaskService(
            ILogger<SubtaskService> logger,
            IPortAuthorityDbContext dbContext,
            ISendEndpointProvider sendEndpointProvider,
            IAssembler<Subtask, SubtaskModel> taskAssembler)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sendEndpointProvider = sendEndpointProvider;
            _taskAssembler = taskAssembler;
        }


        public async Task<IResult<SubtaskModel>> GetTask(Guid taskId)
        {
            if (taskId == Guid.Empty)
            {
                _logger.LogWarning("Subtask ID cannot be empty");
                return Result.BadRequest<SubtaskModel>($"Subtask ID cannot be empty");
            }

            var job = await _dbContext.Tasks
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TaskId == taskId);
            
            return job == null
                ? Result.NotFound<SubtaskModel>($"Subtask not found with ID {taskId}")
                : Result.Ok(_taskAssembler.Assemble(job));
        }

        public async Task<IResult<PagedResult<SubtaskSearchResult>>> ListTasks(SubtaskSearchCriteria criteria, PagingCriteria paging)
        {
            var query = new SubtaskSearchQuery(_dbContext);
            var results = await query.Find(criteria, paging);
            
            return Result.Ok(results);
        }

        public async Task<IResult> CreateTask(CreateSubtaskForm form)
        {
            var jobExists = await _dbContext.Jobs.AnyAsync(x => x.JobId == form.JobId);
            if (!jobExists) 
            {
                _logger.LogWarning("Job does not exist with ID = {JobId}", form.JobId);
                return Result.NotFound($"Job does not exist with ID {form.JobId}");
            }            
            
            var taskExists = await _dbContext.Tasks.AnyAsync(x => x.Job.JobId == form.JobId && x.TaskId == form.TaskId);
            if (taskExists)
            {
                _logger.LogWarning("Subtask already exists with ID = {TaskId}", form.TaskId);
                return Result.Conflict($"Subtask already exists with ID {form.TaskId}");
            }

            await _sendEndpointProvider.Send<CreateSubtask>(new
            {
                JobId = form.JobId,
                TaskId = form.TaskId,
                Name = form.Name
            });
            
            _logger.LogInformation("Sent <{MessageType}> command", nameof(CreateSubtask));
            
            return Result.Ok();
        }

        public async Task<IResult> StartTask(Guid taskId, DateTimeOffset startTime)
        {
            var exists = await _dbContext.Tasks.AnyAsync(x => x.TaskId == taskId);
            if (!exists)
            {
                _logger.LogWarning("Subtask does not exist with ID = {TaskId}", taskId);
                return Result.NotFound($"Subtask does not exist with ID {taskId}");
            }
           
            await _sendEndpointProvider.Send<StartSubtask>(new
            {
                TaskId = taskId,
                StartTime = startTime
            });

            _logger.LogInformation("Sent <{MessageType}> command", nameof(StartSubtask));
            
            return Result.Ok();
        }

        public async Task<IResult> EndTask(Guid taskId, DateTimeOffset endTime, bool success)
        {
            var exists = await _dbContext.Tasks.AnyAsync(x => x.TaskId == taskId);
            if (!exists)
            {
                _logger.LogWarning("Subtask does not exist with ID = {TaskId}", taskId);
                return Result.NotFound($"Subtask does not exist with ID {taskId}");
            }
           
            await _sendEndpointProvider.Send<EndSubtask>(new
            {
                TaskId = taskId,
                EndTime = endTime,
                Success = success
            });

            _logger.LogInformation("Sent <{MessageType}> command", nameof(EndSubtask));
            
            return Result.Ok();
        }
    }
}
