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
using PortAuthority.Results.Validation;

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
        
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(List<JobModel>), StatusCodes.Status200OK)]
        public IActionResult Index([FromQuery] JobSearchCriteria search, [FromQuery] PagingCriteria paging)
        {
            return Ok("success");
        }

        [HttpGet]
        [Route("{taskId}")]
        [ProducesResponseType(typeof(SubtaskModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJob(Guid id)
        {
            var result = await _subtaskService.GetTask(id);

            if (result.IsNotFound())
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(new { Task = result.Payload });
        }
    }
}
