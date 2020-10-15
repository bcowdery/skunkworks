using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PortAuthority.Data.Queries;
using PortAuthority.Forms;
using PortAuthority.Models;
using PortAuthority.Results;

namespace PortAuthority
{
    public interface IJobService
    {
        Task<IResult<JobModel>> GetJob(Guid jobId);
        Task<IResult<PagedResult<JobSearchResult>>> ListJobs(JobSearchCriteria criteria, PagingCriteria paging);

        Task<IResult> CreateJob(CreateJobForm form);
        Task<IResult> StartJob(Guid jobId, DateTimeOffset startTime);
        Task<IResult> EndJob(Guid jobId, DateTimeOffset endTime, bool success);

        Task<IResult<SubtaskModel>> GetTask(Guid taskId);
        IResult<List<SubtaskModel>> ListTasks(Guid jobId);

        Task<IResult> CreateTask(Guid jobId);
        Task<IResult> StartTask(Guid jobId, Guid taskId, DateTimeOffset startTime);
        Task<IResult> EndTask(Guid jobId, Guid taskId, DateTimeOffset endTime, bool success);
    }
}
