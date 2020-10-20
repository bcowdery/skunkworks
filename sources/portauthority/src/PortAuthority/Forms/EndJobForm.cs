using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class EndJobForm 
        : EndJob
    {
        public Guid JobId { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(Success)}: {Success}, {nameof(EndTime)}: {EndTime}";
        }
    }
}
