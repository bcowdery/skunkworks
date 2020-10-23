using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shipyard.Data
{
    public interface IShipyardDbContext
        : IDisposable, IAsyncDisposable
    {
        DatabaseFacade Database { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
    }
}
