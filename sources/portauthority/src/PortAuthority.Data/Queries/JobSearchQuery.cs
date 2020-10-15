using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortAuthority.Data.Entities;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// EntityFramework Query for performing a search of the jobs database.
    /// </summary>
    public class JobSearchQuery
    {
        private readonly IPortAuthorityDbContext _dbContext;

        public JobSearchQuery(IPortAuthorityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Search for jobs matching the given criteria and return a paged list of results.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task<PagedResult<JobResult>> Find(JobSearchCriteria criteria, PagingCriteria paging)
        {
            var query = _dbContext.Jobs.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Type) || !string.IsNullOrEmpty(criteria.Namespace))
            {
                query = query.Where(j => j.Type == criteria.Type && j.Namespace == criteria.Namespace);
            }

            if (criteria.CorrelationId.HasValue)
            {
                query = query.Where(j => j.CorrelationId == criteria.CorrelationId);
            }

            var pageSize = paging.Size;
            var skipRows = (paging.Page - 1) * paging.Size;

            var page = await query
                .OrderBy(j => j.Id)
                .Select(j => new
                {
                    Job = new JobResult
                    {
                        JobId = j.JobId,
                        Type = j.Type,
                        Namespace = j.Namespace,
                        Status = j.Status
                    },
                    TotalCount = query.Count() // note: EF optimizes this so it's only executed once
                })
                .Skip(skipRows).Take(pageSize)
                .ToArrayAsync();
            
            // No results found, return an empty result set
            if (page == null || page.Length == 0)
            {
                return PagedResult<JobResult>.Empty(paging);
            }
            
            // More than one row found!
            // Build the paged result object
            int total = page.First().TotalCount;
            var jobs = page.Select(p => p.Job);
            
            return new PagedResult<JobResult>(paging, total, jobs);
        }
    }
    
    /// <summary>
    /// Job search result
    /// </summary>
    public class JobResult
    {
        public Guid JobId { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
        public Status Status { get; set; }
    }
}
