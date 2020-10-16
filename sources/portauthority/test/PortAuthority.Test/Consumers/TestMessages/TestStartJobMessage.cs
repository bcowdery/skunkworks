using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    /// <summary>
    /// Test implementation of <see cref="StartJob"/>.
    /// </summary>
    internal sealed class TestStartJobMessage 
        : StartJob
    {
        public Guid JobId { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}
