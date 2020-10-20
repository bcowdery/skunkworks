using System;
using System.Collections.Generic;
using PortAuthority.Data.Entities;

namespace PortAuthority.Models
{
    /// <summary>
    /// A sub-task of a tracked job
    /// </summary>
    public class SubtaskModel
    {
        /// <summary>
        /// Task ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Sub-task name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Sub-task status
        /// </summary>
        public Status Status { get; set; }
        
        /// <summary>
        /// Start time of the task (null if not started).
        /// </summary>
        public DateTimeOffset? StartTime { get; set; } 
        
        /// <summary>
        /// End time of the task (null if not started or in-progress).
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }
        
        /// <summary>
        /// Metadata
        /// </summary>
        public Dictionary<string, object> Meta { get; set; }
    }
}
