using Microsoft.Data.SqlClient;

namespace PortAuthority.Forms
{
    public class PagingCriteria
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string Sort { get; set; }
        public SortOrder Order { get; set; }
    }
}
