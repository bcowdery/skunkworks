using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [HttpGet, Route("", Name = nameof(Index))]
        [ProducesResponseType(typeof(List<JobModel>), StatusCodes.Status200OK)]
        public IActionResult Index([FromQuery] JobSearchCriteria search, [FromQuery] PagingCriteria paging)
        {
            return Ok("success");
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
            var result = await _jobService.GetJob(id);

            if (result.IsNotFound())
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(new { Job = result.Payload });
        }

        /// <summary>
        /// Creates a new Job
        /// </summary>
        /// <param name="createJob"></param>
        /// <returns></returns>
        [HttpPost, Route("{id}", Name = nameof(CreateJob))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobForm createJob)
        {
            var result = await _jobService.CreateJob(createJob);

            if (result.IsConflict())
            {
                return Conflict(result.ErrorMessage);
            }

            var url = Url.RouteUrl(nameof(GetJob), new { Id = createJob.JobId });
            return Accepted(JsonLinks.Self(url));
        }
    }
}
