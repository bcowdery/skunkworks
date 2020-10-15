using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PortAuthority.Web.Results
{
    /// <summary>
    /// HTTP 404 NotFound result with a JSON message payload.
    /// </summary>
    public class NotFoundJsonResult : JsonResult
    {
        public NotFoundJsonResult(string message) 
            : base(new JsonMessage(message))
        {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }
}