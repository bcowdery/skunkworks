using System;

namespace PortAuthority.Forms
{
    public class JobSearchCriteria
    {
        public Guid? CorrelationId { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
    }
}
