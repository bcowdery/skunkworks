using System.Net;
using Shipyard.Results.Errors;
using Shipyard.Results.Validation;

namespace Shipyard.Results
{
    /// <summary>
    /// Result of an API operation.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Desired HTTP Response Code 
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Error message 
        /// </summary>
        ErrorMessage ErrorMessage { get; }

        /// <summary>
        /// Validation result
        /// </summary>
        ValidationResult ValidationResult { get; }
    }
}
