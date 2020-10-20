using System;
using System.Collections.Generic;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    /// <summary>
    /// Test implementation of <see cref="CreateJob"/>.
    /// </summary>
    internal sealed class TestCreateJobMessage 
        : CreateJob
    {
        public Guid JobId { get; set; }
        public Guid? CorrelationId { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
        public Dictionary<string, object> Meta { get; set; }
    }
}
