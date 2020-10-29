using System.ComponentModel;

namespace Shipyard.Contracts.MessageTypes
{
    public enum MessageType
    {
        [Description("Manually sent email")]
        Manual = 0,
        
        [Description("Automated transactional message to be sent immediately.")]
        Transactional = 1,
        
        [Description("Scheduled message to load balanced.")]
        Scheduled = 2
    }
}
