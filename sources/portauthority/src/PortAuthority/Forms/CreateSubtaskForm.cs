using System;
using System.Collections.Generic;
using System.Text;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class CreateSubtaskForm
        : CreateSubtask
    {
        public Guid JobId { get; set; }
        public Guid TaskId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{nameof(JobId)}: {JobId}, {nameof(TaskId)}: {TaskId}, {nameof(Name)}: {Name}";
        }
    }
}
