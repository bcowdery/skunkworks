using System.Net;
using PortAuthority.Results.Errors;
using PortAuthority.Results.Validation;

namespace PortAuthority.Results
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
