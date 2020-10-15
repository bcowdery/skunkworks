using System;
using System.ComponentModel;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// Search criteria for finding jobs.
    /// </summary>
    public class JobSearchCriteria
    {
        public Guid? CorrelationId { get; set; }
        
        /// <summary>
        /// Type of job to search for
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Namespace of the job to search for (both Type & Namespace are required 
        /// </summary>
        public string Namespace { get; set; }
    }
}
