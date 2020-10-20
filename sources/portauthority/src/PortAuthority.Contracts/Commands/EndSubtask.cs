using System;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Ends a task
    /// </summary>
    public interface EndSubtask
    {
        Guid TaskId { get; }
        bool Success { get; }
        DateTimeOffset EndTime { get; }
    }
}
