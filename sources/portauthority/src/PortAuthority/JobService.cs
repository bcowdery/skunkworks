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
using PortAuthority.Forms;
using PortAuthority.Models;
using PortAuthority.Results;

namespace PortAuthority
{
    public class JobService
        : IJobService
    {
        private readonly ILogger<JobService> _logger;
        private readonly IPortAuthorityDbContext _dbContext;
        private readonly ISendEndpoint _sendEndpoint;
        private readonly IAssembler<Job, JobModel> _jobAssembler;
        
        public JobService(
            ILogger<JobService> logger,
            IPortAuthorityDbContext dbContext,
            ISendEndpoint sendEndpoint,
            IAssembler<Job, JobModel> jobAssembler)
        {
            _logger = logger;
            _jobAssembler = jobAssembler;
            _dbContext = dbContext;
            _sendEndpoint = sendEndpoint;
        }

        public async Task<IResult<JobModel>> GetJob(Guid jobId)
        {
            if (jobId == Guid.Empty)
            {
                return Result.BadRequest<JobModel>($"Job ID cannot be empty");
            }

            var job = await _dbContext.Jobs.SingleOrDefaultAsync(x => x.JobId == jobId);

            return job == null
                ? Result.NotFound<JobModel>($"Job not found with ID {jobId}")
                : Result.Ok(_jobAssembler.Assemble(job));
        }

        public IResult<List<JobModel>> ListJobs(JobSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> CreateJob(CreateJobForm form)
        {
            var exists = await _dbContext.Jobs.AnyAsync(x => x.JobId == form.JobId);
            if (exists)
            {
                return Result.Conflict($"Job already exists with ID {form.JobId}");
            }

            await _sendEndpoint.Send<CreateJob>(new
            {
                JobId = form.JobId,
                CorrelationId = form.JobId,
                Type = form.Type,
                Namespace = form.Namespace
            });

            return Result.Ok();
        }

        public async Task<IResult> StartJob(Guid jobId, DateTimeOffset startTime)
        {
            await _sendEndpoint.Send<StartJob>(new
            {
                JobId = jobId,
                StartTime = startTime
            });

            return Result.Ok();
        }

        public async Task<IResult> EndJob(Guid jobId, DateTimeOffset endTime, bool success)
        {
            await _sendEndpoint.Send<EndJob>(new
            {
                JobId = jobId,
                EndTime = endTime,
                Success = success
            });

            return Result.Ok();
        }

        public Task<IResult<SubtaskModel>> GetTask(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public IResult<List<SubtaskModel>> ListTasks(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> CreateTask(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> StartTask(Guid jobId, Guid taskId, DateTimeOffset startTime)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> EndTask(Guid jobId, Guid taskId, DateTimeOffset endTime, bool success)
        {
            throw new NotImplementedException();
        }
    }
}
