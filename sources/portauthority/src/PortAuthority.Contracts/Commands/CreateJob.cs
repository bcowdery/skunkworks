using System;
using System.Collections.Generic;

namespace PortAuthority.Contracts.Commands
{
    /// <summary>
    /// Creates a new job in a Pending state
    /// </summary>
    public interface CreateJob 
    {
        /// <summary>
        /// Job ID
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Optional correlation ID to associate multiple jobs together.
        /// </summary>
        Guid? CorrelationId { get; }

        /// <summary>
        /// Job type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Job namespace
        /// </summary>
        string Namespace { get; }
        
        /// <summary>
        /// Additional job metadata
        /// </summary>
        Dictionary<string,object> Meta { get; }
    }
}
