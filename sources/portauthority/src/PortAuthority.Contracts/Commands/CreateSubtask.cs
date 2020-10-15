using System;
using System.Collections.Generic;
using System.Text;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Creates a new task in a pending state
    /// </summary>
    public interface CreateSubtask
    {
        /// <summary>
        /// Job ID
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Task ID
        /// </summary>
        Guid TaskId { get; }
        
        /// <summary>
        /// Task name
        /// </summary>
        string Name { get; }
    }
}
