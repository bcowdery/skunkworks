using System.Collections.Generic;
using System.Linq;

namespace Shipyard.Results.Validation
{
    /// <summary>
    /// Result of an API request validation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Validation errors. Empty if no issues found.
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

        /// <summary>
        /// Returns true if the request is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => Errors == null || !Errors.Any();
        
        public override string ToString()
        {
            return string.Join(",",Errors.ToString());
        }
    }
}
