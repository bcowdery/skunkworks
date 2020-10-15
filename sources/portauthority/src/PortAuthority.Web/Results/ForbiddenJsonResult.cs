using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PortAuthority.Web.Results
{
    /// <summary>
    /// HTTP 403 Forbidden result with a JSON message payload.
    /// </summary>
    public class ForbiddenJsonResult : JsonResult
    {
        public ForbiddenJsonResult(string message)
            : base(new JsonMessage(message))
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}
