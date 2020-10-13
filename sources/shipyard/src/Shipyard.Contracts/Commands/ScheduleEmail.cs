namespace Shipyard.Contracts.Commands 
{
    public interface ScheduleEmail
    {
        IEmail Email { get; }
        ISchedule Schedule { get; }        
    }
}
