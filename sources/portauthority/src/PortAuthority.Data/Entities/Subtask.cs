using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortAuthority.Data.Entities
{
    [Table("Subtasks")]
    public class Subtask
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id { get; set; }

        /// <summary>
        /// Parent job ID
        /// </summary>
        public long JobId { get; set; }
        public Job Job { get; set; }
        
        /// <summary>
        /// Sub-task ID. Use of sequential GUID's is recommended for performance.
        /// <example><code>NewId.NextGuid()</code></example>
        /// </summary>
        [Required]
        public Guid TaskId { get; set; }
        
        /// <summary>
        /// Job type
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
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
        /// Metadata
        /// </summary>
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();
    }
}
