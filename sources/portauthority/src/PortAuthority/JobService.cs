using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Job service for handling REST API requests
    /// </summary>
    public class JobService
        : IJobService
    {
        private readonly ILogger<JobService> _logger;
        private readonly IPortAuthorityDbContext _dbContext;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IAssembler<Job, JobModel> _jobAssembler;
        
        
        public JobService(
            ILogger<JobService> logger,
            IPortAuthorityDbContext dbContext,
            ISendEndpointProvider sendEndpointProvider,
            IAssembler<Job, JobModel> jobAssembler)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sendEndpointProvider = sendEndpointProvider;
            _jobAssembler = jobAssembler;
        }

        
        public async Task<IResult<JobModel>> GetJob(Guid jobId)
        {
            if (jobId == Guid.Empty)
            {
                _logger.LogWarning("Job ID cannot be empty");
                return Result.BadRequest<JobModel>($"Job ID cannot be empty");
            }

            var job = await _dbContext.Jobs
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.JobId == jobId);

            // TODO: Add the SubtaskSummaryModel to the query and JobModel
            
            return job == null
                ? Result.NotFound<JobModel>($"Job not found with ID {jobId}")
                : Result.Ok(_jobAssembler.Assemble(job));
        }

        public async Task<IResult<PagedResult<JobSearchResult>>> ListJobs(JobSearchCriteria criteria, PagingCriteria paging)
        {
            var query = new JobSearchQuery(_dbContext);
            var results = await query.Find(criteria, paging);
            
            return Result.Ok(results);
        }

        public async Task<IResult> CreateJob(CreateJobForm form)
        {
            var exists = await _dbContext.Jobs.AnyAsync(x => x.JobId == form.JobId);
            if (exists)
            {
                _logger.LogWarning("Job already exists with ID = {JobId}", form.JobId);
                return Result.Conflict($"Job already exists with ID {form.JobId}");
            }
            
            await _sendEndpointProvider.Send<CreateJob>(new
            {
                JobId = form.JobId,
                CorrelationId = form.CorrelationId,
                Type = form.Type,
                Namespace = form.Namespace,
                Meta = form.Meta
            });

            _logger.LogInformation("Sent <{MessageType}> command", nameof(CreateJob));
            
            return Result.Ok();
        }

        public async Task<IResult> StartJob(Guid jobId, DateTimeOffset startTime)
        {
            var exists = await _dbContext.Jobs.AnyAsync(x => x.JobId == jobId);
            if (!exists)
            {
                _logger.LogWarning("Job does not exist with ID = {JobId}", jobId);
                return Result.NotFound($"Job does not exist with ID {jobId}");
            }
            
            await _sendEndpointProvider.Send<StartJob>(new
            {
                JobId = jobId,
                StartTime = startTime
            });

            _logger.LogInformation("Sent <{MessageType}> command", nameof(StartJob));
            
            return Result.Ok();
        }

        public async Task<IResult> EndJob(Guid jobId, DateTimeOffset endTime, bool success)
        {
            var exists = await _dbContext.Jobs.AnyAsync(x => x.JobId == jobId);
            if (!exists)
            {
                _logger.LogWarning("Job does not exist with ID = {JobId}", jobId);
                return Result.NotFound($"Job does not exist with ID {jobId}");
            }
           
            await _sendEndpointProvider.Send<EndJob>(new
            {
                JobId = jobId,
                EndTime = endTime,
                Success = success
            });

            _logger.LogInformation("Sent <{MessageType}> command", nameof(EndJob));
            
            return Result.Ok();
        }
    }
}
