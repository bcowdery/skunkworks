using PortAuthority.Data.Entities;
using PortAuthority.Models;

namespace PortAuthority.Assemblers
{
    public class JobModelAssembler
        : IAssembler<Job, JobModel>
    {
        public JobModel Assemble(Job arg)
        {
            if (arg == null) return null;

            return new JobModel()
            {
                JobId = arg.JobId,
                CorrelationId = arg.CorrelationId,
                Type = arg.Type,
                Namespace = arg.Namespace,
                StartTime = arg.StartTime,
                EndTime = arg.EndTime,
                Tasks = new SubtaskSummaryModel(),
                Meta = arg.Meta
            };
        }
    }
}
