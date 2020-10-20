using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class CreateSubtaskForm
        : CreateSubtask
    {
        [Required]
        public Guid JobId { get; set; }
        
        [Required]
        public Guid TaskId { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(TaskId)}: {TaskId}, {nameof(Name)}: {Name}";
        }
    }
}
