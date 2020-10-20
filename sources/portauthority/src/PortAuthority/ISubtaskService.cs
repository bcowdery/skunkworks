using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortAuthority.Data.Queries;
using PortAuthority.Forms;
using PortAuthority.Models;
using PortAuthority.Results;

namespace PortAuthority
{
    public interface ISubtaskService
    {
     
        Task<IResult<SubtaskModel>> GetTask(Guid taskId);
        Task<IResult<PagedResult<SubtaskSearchResult>>> ListTasks(SubtaskSearchCriteria criteria, PagingCriteria paging);

        Task<IResult> CreateTask(CreateSubtaskForm form);
        Task<IResult> StartTask(Guid taskId, DateTimeOffset startTime);
        Task<IResult> EndTask(Guid taskId, DateTimeOffset endTime, bool success);
    }
}
