using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    public class TestEndJobMessage 
        : EndJob
    {
        public Guid JobId { get; set;  }
        public bool Success { get; set; }
        public DateTimeOffset EndTime { get; set;  }
    }
}
