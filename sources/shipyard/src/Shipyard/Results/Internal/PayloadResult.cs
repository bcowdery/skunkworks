using System.Net;
using Shipyard.Results.Errors;
using Shipyard.Results.Validation;

namespace Shipyard.Results.Internal
{
    /// <summary>
    /// API result with a JSON response payload
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal sealed class PayloadResult<TPayload> :
        IResult<TPayload>
    {
        public HttpStatusCode StatusCode { get; }
        public ErrorMessage ErrorMessage { get; }
        public ValidationResult ValidationResult { get; } = new ValidationResult();
        public TPayload Payload { get; }

        
        public PayloadResult(HttpStatusCode statusCode, TPayload payload)
        {
            StatusCode = statusCode;
            Payload = payload;
        }
        
        public PayloadResult(HttpStatusCode statusCode, TPayload payload, ValidationResult validation)
        {
            StatusCode = statusCode;
            Payload = payload;
            ValidationResult = validation;
        }

        public PayloadResult(HttpStatusCode statusCode, ErrorMessage errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
