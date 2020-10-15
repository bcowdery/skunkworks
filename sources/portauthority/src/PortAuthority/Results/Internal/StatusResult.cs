using System.Net;
using PortAuthority.Results.Errors;
using PortAuthority.Results.Validation;

namespace PortAuthority.Results.Internal
{
    /// <summary>
    /// API result without content
    /// </summary>
    internal sealed class StatusResult : IResult
    {
        public HttpStatusCode StatusCode { get; }
        public ErrorMessage ErrorMessage { get; }
        public ValidationResult ValidationResult { get; } = new ValidationResult();

        public StatusResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public StatusResult(HttpStatusCode statusCode, ValidationResult validation)
        {
            StatusCode = statusCode;
            ValidationResult = validation;
        }

        public StatusResult(HttpStatusCode statusCode, ErrorMessage errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}

