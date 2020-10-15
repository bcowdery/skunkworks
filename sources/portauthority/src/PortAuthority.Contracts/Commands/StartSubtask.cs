using System;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Starts a sub-task
    /// </summary>
    public interface StartSubtask
    {
        /// <summary>
        /// Task ID
        /// </summary>
        Guid TaskId { get; }
        
        /// <summary>
        /// Start time of the sub-task
        /// </summary>
        DateTimeOffset StartTime { get; }
    }
}
