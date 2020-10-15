using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PortAuthority.Web.Results
{
    /// <summary>
    /// HTTP 401 Unauthorized result with a JSON message payload.
    /// </summary>
    public class UnauthorizedJsonResult : JsonResult
    {
        public UnauthorizedJsonResult(string message) 
            : base(new JsonMessage(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}