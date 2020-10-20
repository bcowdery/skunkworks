using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    public class TestStartSubtaskMessage
        : StartSubtask
    {
        public Guid TaskId { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}
