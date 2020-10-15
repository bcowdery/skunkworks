using Microsoft.Data.SqlClient;

namespace PortAuthority.Data.Queries
{
    public class PagingCriteria
    {
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
