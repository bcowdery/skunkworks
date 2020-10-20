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
using PortAuthority.Results.Validation;
using PortAuthority.Web.Results;

namespace PortAuthority.Web.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class SubtaskController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly ISubtaskService _subtaskService;

        public SubtaskController(ILogger<JobController> logger, ISubtaskService subtaskService)
        {
            _logger = logger;
            _subtaskService = subtaskService;
        }

        /// <summary>
        /// List all sub-tasks
        /// </summary>
        /// <param name="search"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet, Route("", Name = nameof(ListTasks))]
        [ProducesResponseType(typeof(PagedResult<SubtaskSearchResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListTasks([FromQuery] SubtaskSearchCriteria search, [FromQuery] PagingCriteria paging)
        {
            _logger.LogInformation("Finding all jobs for criteria = [{Criteria}]", search);
            _logger.LogInformation("Page  = [{Paging}]", paging);
            
            var result = await _subtaskService.ListTasks(search, paging);
            return Ok(result.Payload);
        }

        /// <summary>
        /// Returns a sub-task by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}", Name = nameof(GetTask))]
        [ProducesResponseType(typeof(SubtaskModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var result = await _subtaskService.GetTask(id);

            if (result.IsNotFound())
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(new { Task = result.Payload });
        }
        
        /// <summary>
        /// Creates a new sub-task
        /// </summary>
        /// <param name="createTask"></param>
        /// <returns></returns>
        [HttpPost, Route("", Name = nameof(CreateTask))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateTask([FromBody] CreateSubtaskForm createTask)
        {
            _logger.LogInformation("Creating sub-task = [{Task}]", createTask);

            var result = await _subtaskService.CreateTask(createTask);

            if (result.IsConflict())
            {
                _logger.LogWarning("Task already exists with Id = {TaskId}", createTask.TaskId);
                return Conflict(result.ErrorMessage);
            }

            var url = Url.RouteUrl(nameof(GetTask), new { Id = createTask.JobId });
            return Accepted(JsonLinks.Self(url));
        }


        /// <summary>
        /// Record the start time of a sub-task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTask"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/start", Name = nameof(StartTask))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> StartTask(Guid id, [FromBody] StartSubtaskForm startTask)
        {
            _logger.LogInformation("Starting sub-task = [TaskId = {TaskId}, {Job}]", id, startTask);
            
            var result = await _subtaskService.StartTask(id, startTask.StartTime);

            if (result.IsNotFound())
            {
                _logger.LogWarning("Sub-task does not exist with Id = {TaskId}", id);
                return Conflict(result.ErrorMessage);
            }
            
            var url = Url.RouteUrl(nameof(GetTask), new { Id = id });
            return Accepted(JsonLinks.Self(url));
        }

        /// <summary>
        /// Record the end time of a sub-task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endTask"></param>
        /// <returns></returns>
        [HttpPut, Route("{id}/end", Name = nameof(EndTask))]
        [ProducesResponseType(typeof(JsonLinks), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> EndTask(Guid id, [FromBody] EndSubtaskForm endTask)
        {
            _logger.LogInformation("Ending sub-task = [TaskId = {TaskId}, {Job}]", id, endTask);
            
            var result = await _subtaskService.EndTask(id, endTask.EndTime, endTask.Success);

            if (result.IsNotFound())
            {
                _logger.LogWarning("Sub-task does not exist with Id = {JobId}", id);
                return Conflict(result.ErrorMessage);
            }
            
            var url = Url.RouteUrl(nameof(GetTask), new { Id = id });
            return Accepted(JsonLinks.Self(url));
        }                
    }
}
