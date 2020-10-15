using System;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Starts a job
    /// </summary>
    public interface StartJob 
    {
        /// <summary>
        /// Job ID
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Start time of the job
        /// </summary>
        DateTimeOffset StartTime { get; }
    }
}
