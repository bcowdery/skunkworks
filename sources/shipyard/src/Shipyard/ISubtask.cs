using System;

namespace Shipyard
{
    public interface ISubtask
    {
        public Guid? Id { get; set; }
        public Guid? TaskId { get; set; }
    }
}
