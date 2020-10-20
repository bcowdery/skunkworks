using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    /// <summary>
    /// Test implementation of <see cref="EndSubtask"/>
    /// </summary>
    public class TestEndSubtaskMessage
        : EndSubtask
    {
        public Guid TaskId { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
