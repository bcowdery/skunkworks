using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortAuthority.Data.Entities
{
    [Table("Jobs")]
    public class Job
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Job ID. Use of sequential GUIDs are recommended for performance.
        /// <example><code>NewId.NextGuid()</code></example>
        /// </summary>
        [Required]
        public Guid JobId { get; set; }

        /// <summary>
        /// Optional correlation ID to associate multiple jobs together.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Job type
        /// </summary>
        [Required, MaxLength(100)]
        public string Type { get; set; }

        /// <summary>
        /// Job namespace
        /// </summary>
        [Required, MaxLength(100)]
        public string Namespace { get; set; }

        /// <summary>
        /// Job status
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Start time of the job (null if not started).
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// End time of the job (null if not started or in-progress).
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }        

        /// <summary>
        /// Job sub-tasks 
        /// </summary>
        public List<Subtask> Tasks { get; set; } = new List<Subtask>();

        /// <summary>
        /// Metadata
        /// </summary>
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        
        public bool IsPending()
        {
            return Status == Status.Pending;
        }

        public bool IsRunning()
        {
            return Status == Status.InProgress;
        }

        public bool IsFinished()
        {
            return Status == Status.Failed || Status == Status.Completed;
        }
    }
}
