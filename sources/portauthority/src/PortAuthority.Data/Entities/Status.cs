using System.ComponentModel;

namespace PortAuthority.Data.Entities
{
    /// <summary>
    /// Status of a job or sub-task
    /// </summary>
    public enum Status 
    {
        [Description("Pending")]
        Pending = 0,

        [Description("In Progress")]
        InProgress = 1,
        
        [Description("Failed")]
        Failed = 2,
        
        [Description("Complete")]
        Completed = 3
    }
}
