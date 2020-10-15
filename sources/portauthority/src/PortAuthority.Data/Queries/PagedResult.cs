using System.Collections.Generic;
using System.Linq;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// Paged query result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        private PagedResult()
        {
        }
        
        public PagedResult(PagingCriteria paging, int total, IEnumerable<T> data)
        {
            Page = paging.Page;
            Size = paging.Size;
            TotalItems = total;
            TotalPages = total <= 0 ? 0 : total <= paging.Size ? 1 : (total / paging.Size);
            Data = data as T[] ?? data.ToArray();
        }

        public static PagedResult<T> Empty(PagingCriteria paging)
        {
            return new PagedResult<T>()
            {
                Page = paging.Page,
                Size = paging.Size,
                TotalItems = 0,
                TotalPages = 0
            };
        }
        
        public int Page { get; private set; }
        public int Size { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }
        public T[] Data { get; private set; }
    }
}
