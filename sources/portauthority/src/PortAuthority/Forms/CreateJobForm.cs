using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class CreateJobForm
        : CreateJob
    {
        [Required]
        public Guid JobId { get; set; }
        public Guid? CorrelationId { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public string Namespace { get; set; }

        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(CorrelationId)}: {CorrelationId}, {nameof(Type)}: {Type}, {nameof(Namespace)}: {Namespace}";
        }
    }
}
