using PortAuthority.Data.Entities;
using PortAuthority.Models;

namespace PortAuthority.Assemblers
{
    public class SubtaskModelAssembler
        : IAssembler<Subtask, SubtaskModel>
    {
        public SubtaskModel Assemble(Subtask arg)
        {
            if (arg == null) return null;

            return new SubtaskModel()
            {
                TaskId = arg.TaskId,
                Name = arg.Name,
                Status = arg.Status,
                StartTime = arg.StartTime,
                EndTime = arg.EndTime,
                Meta = arg.Meta
            };
        } 
    }
}
