using System.Threading;
using System.Threading.Tasks;
using MassTransit.Registration;
using Microsoft.Extensions.Hosting;

public class MassTransitHostedService 
    : IHostedService
{
    private readonly IBusDepot _depot;
    private Task _startTask;

    public MassTransitHostedService(IBusDepot depot)
    {
        _depot = depot;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _startTask = _depot.Start(cancellationToken);

        return _startTask.IsCompleted
            ? _startTask
            : Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _depot.Stop(cancellationToken);
    }
}
