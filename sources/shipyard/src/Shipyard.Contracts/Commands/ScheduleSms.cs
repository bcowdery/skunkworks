namespace Shipyard.Contracts.Commands 
{
    public interface ScheduleSms
    {
        ISms Sms { get; }
        ISchedule Schedule { get; }        
    }
}
