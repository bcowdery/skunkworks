using Microsoft.Data.SqlClient;

namespace PortAuthority.Data.Queries
{
    /// <summary>
    /// Paging criteria for search operations
    /// </summary>
    public class PagingCriteria
    {
        /// <summary>
        /// Page number
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int Size { get; set; } = 25;

        public override string ToString()
        {
            return $"{nameof(Page)}: {Page}, {nameof(Size)}: {Size}";
        }
    }
}
