using System;
using System.Collections.Generic;

namespace PortAuthority.Models
{
    /// <summary>
    /// A tracked job
    /// </summary>
    public class JobModel
    {
        /// <summary>
        /// Job ID
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Optional correlation ID to associate multiple jobs together.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Job type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Job namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Start time of the job (null if not started).
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// End time of the job (null if not started or in-progress).
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }        

        /// <summary>
        /// Summary of running sub-tasks
        /// </summary>
        public SubtaskSummaryModel Tasks { get; set; }

        /// <summary>
        /// Metadata
        /// </summary>
        public Dictionary<string, object> Meta { get; set; }
    }
}
