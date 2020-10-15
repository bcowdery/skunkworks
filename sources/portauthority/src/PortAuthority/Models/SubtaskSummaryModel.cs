using System;

namespace PortAuthority.Models
{
    /// <summary>
    /// Summary of job sub-tasks
    /// </summary>
    public class SubtaskSummaryModel 
    {
        /// <summary>
        /// Total number of sub-tasks
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Count of pending sub-tasks
        /// </summary>
        public int Pending { get; set; }

        /// <summary>
        /// Count of in-progress sub-tasks
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        /// Count of failed sub-tasks
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Count of completed sub-tasks
        /// </summary>
        public int Completed { get; set; }        
    }
}
