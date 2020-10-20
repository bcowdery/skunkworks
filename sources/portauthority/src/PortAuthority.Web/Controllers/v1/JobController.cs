using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data.Queries;
using PortAuthority.Extensions;
using PortAuthority.Forms;
using PortAuthority.Models;
using PortAuthority.Results.Errors;
using PortAuthority.Web.Results;

namespace PortAuthority.Web.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class JobController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobService _jobService;

        public JobController(ILogger<JobController> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }
        
        /// <summary>
        /// Lists all jobs
        /// </summary>
        /// <param name="search"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet, Route("", Name = nameof(ListJobs))]
        [ProducesResponseType(typeof(PagedResult<JobSearchResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListJobs([FromQuery] JobSearchCriteria search, [FromQuery] PagingCriteria paging)
        {
            _logger.LogInformation("Finding all jobs for criteria = [{Criteria}]", search);
            _logger.LogInformation("Page  = [{Paging}]", paging);
            
            var result = await _jobService.ListJobs(search, paging);
            return Ok(result.Payload);
        }

        /// <summary>
        /// Returns a Job by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}", Name = nameof(GetJob))]
        [ProducesResponseType(typeof(JobModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJob(Guid id)
        {
            _logger.LogInformation("Get job Id = {JobId}", id);
            
            var result = await _jobService.GetJob(id);

            if (result.IsNotFound())
            {
                _logger.LogWarning("Job not found with Id = {JobId}", id);
                return NotFound(result.ErrorMessage);
            }

            return Ok(new { Job = result.Payload });
        }

        /// <summary>
        /// Creates a new Job
        /// </summary>
        /// <param name="createJob"></param>
        /// <returns></returns>
        [HttpPost, Route("", Name = nameof(CreateJob))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobForm createJob)
        {
            _logger.LogInformation("Creating job = [{Job}]", createJob);
            
            var result = await _jobService.CreateJob(createJob);

            if (result.IsConflict())
            {
                _logger.LogWarning("Job already exists with Id = {JobId}", createJob.JobId);
                return Conflict(result.ErrorMessage);
            }

            var url = Url.RouteUrl(nameof(GetJob), new { Id = createJob.JobId });
            return Accepted(JsonLinks.Self(url));
        }

        /// <summary>
        /// Record the start time of a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startJob"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/start", Name = nameof(StartJob))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> StartJob(Guid id, [FromBody] StartJobForm startJob)
        {
            _logger.LogInformation("Starting job = [JobId = {JobId}, {Job}]", id, startJob);
            
            var result = await _jobService.StartJob(id, startJob.StartTime);

            if (result.IsNotFound())
            {
                _logger.LogWarning("Job does not exist with Id = {JobId}", id);
                return Conflict(result.ErrorMessage);
            }
            
            var url = Url.RouteUrl(nameof(GetJob), new { Id = id });
            return Accepted(JsonLinks.Self(url));
        }

        /// <summary>
        /// Record the end time of a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endJob"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/end", Name = nameof(EndJob))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> EndJob(Guid id, [FromBody] EndJobForm endJob)
        {
            _logger.LogInformation("Ending job = [JobId = {JobId}, {Job}]", id, endJob);
            
            var result = await _jobService.EndJob(id, endJob.EndTime, endJob.Success);

            if (result.IsNotFound())
            {
                _logger.LogWarning("Job does not exist with Id = {JobId}", id);
                return Conflict(result.ErrorMessage);
            }
            
            var url = Url.RouteUrl(nameof(GetJob), new { Id = id });
            return Accepted(JsonLinks.Self(url));
        }        
    }
}
