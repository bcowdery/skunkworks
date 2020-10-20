using System;
using System.Collections.Generic;
using System.Text;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class CreateJobForm
        : CreateJob
    {
        public Guid JobId { get; set; }
        public Guid? CorrelationId { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(CorrelationId)}: {CorrelationId}, {nameof(Type)}: {Type}, {nameof(Namespace)}: {Namespace}";
        }
    }
}
