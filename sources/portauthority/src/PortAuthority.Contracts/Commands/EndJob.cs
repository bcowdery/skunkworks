using System;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Ends a job
    /// </summary>
    public interface EndJob 
    {
        /// <summary>
        /// Job ID
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Completion status of the job (success or failure).
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Completion time of the job
        /// </summary>
        DateTimeOffset EndTime { get; }
    }
}
