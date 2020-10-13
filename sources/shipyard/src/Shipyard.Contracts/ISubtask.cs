using System;

namespace Shipyard.Contracts
{
    public interface ISubtask
    {
        Guid? Id { get; }
        Guid? TaskId { get; }
    }
}
