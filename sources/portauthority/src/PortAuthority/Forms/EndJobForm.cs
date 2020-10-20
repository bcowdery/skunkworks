using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class StartJobForm 
        : StartJob
    {
        public Guid JobId { get; set;  }
        public DateTimeOffset StartTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(StartTime)}: {StartTime}";
        }
    }
}
