using System;
using System.ComponentModel;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// Search criteria for finding jobs.
    /// </summary>
    public class JobSearchCriteria
    {
        /// <summary>
        /// Optional correlation ID to find related jobs
        /// </summary>
        public Guid? CorrelationId { get; set; }
        
        /// <summary>
        /// Type of job to search for
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Namespace of the job to search for (both Type & Namespace are required 
        /// </summary>
        public string Namespace { get; set; }

        public override string ToString()
        {
            return $"{nameof(CorrelationId)}: {CorrelationId}, {nameof(Type)}: {Type}, {nameof(Namespace)}: {Namespace}";
        }
    }
}
