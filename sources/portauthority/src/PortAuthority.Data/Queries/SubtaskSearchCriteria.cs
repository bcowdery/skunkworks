using System;
using System.ComponentModel;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// Search criteria for finding tasks.
    /// </summary>
    public class SubtaskSearchCriteria
    {
        /// <summary>
        /// Parent Job ID
        /// </summary>
        public Guid? JobId { get; set; }
        
        /// <summary>
        /// Sub-task names to search for
        /// </summary>
        public string Name { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(Name)}: {Name}";
        }
    }
}
