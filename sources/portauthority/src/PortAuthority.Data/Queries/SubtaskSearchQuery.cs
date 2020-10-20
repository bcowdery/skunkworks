using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortAuthority.Data.Entities;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// EntityFramework Query for performing a search of the subtask database.
    /// </summary>
    public class SubtaskSearchQuery
    {
        private readonly IPortAuthorityDbContext _dbContext;

        public SubtaskSearchQuery(IPortAuthorityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Search for tasks matching the given criteria and return a paged list of results.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task<PagedResult<SubtaskSearchResult>> Find(SubtaskSearchCriteria criteria, PagingCriteria paging)
        {
            if (paging.Page <= 0 || paging.Size <= 0)
            {
                return PagedResult<SubtaskSearchResult>.Empty(paging);
            }

            // Build search query
            var query = _dbContext.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(j => j.Name == criteria.Name);
            }

            if (criteria.JobId.HasValue)
            {
                query = query.Where(j => j.Job.JobId == criteria.JobId);
            }

            var pageSize = paging.Size;
            var skipRows = (paging.Page - 1) * paging.Size;

            var page = await query
                .OrderBy(t => t.Id)
                .Select(t => new
                {
                    Job = new SubtaskSearchResult
                    {
                        JobId = t.Job.JobId,
                        TaskId = t.TaskId,
                        Name = t.Name,
                        Status = t.Status
                    },
                    TotalCount = query.Count() // note: EF optimizes this so it's only executed once
                })
                .Skip(skipRows).Take(pageSize)
                .ToArrayAsync();
            
            // No results found, return an empty result set
            if (page == null || page.Length == 0)
            {
                return PagedResult<SubtaskSearchResult>.Empty(paging);
            }
            
            // More than one row found!
            // Build the paged result object
            int total = page.First().TotalCount;
            var jobs = page.Select(p => p.Job);
            
            return new PagedResult<SubtaskSearchResult>(paging, total, jobs);
        }
    }
    
    /// <summary> 
    /// Subtask search result
    /// </summary>
    public class SubtaskSearchResult
    {
        public Guid JobId { get; set; }
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
    }
}
