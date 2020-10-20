using System;
using System.Collections.Generic;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Test.Consumers.TestMessages
{
    /// <summary>
    /// Test implementation of <see cref="CreateSubtask"/>.
    /// </summary>    
    public class TestCreateSubtaskMessage
        : CreateSubtask
    {
        public Guid JobId { get; set; }
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Meta { get; set; }
    }
}
