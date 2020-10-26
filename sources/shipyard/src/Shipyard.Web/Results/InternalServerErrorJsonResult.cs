using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shipyard.Web.Results
{
    /// <summary>
    /// HTTP 500 InternalServerError result with a JSON message payload.
    /// </summary>
    public class InternalServerErrorJsonResult : JsonResult
    {
        public InternalServerErrorJsonResult(string message) 
            : base(new JsonMessage(message))
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}