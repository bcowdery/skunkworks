using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PortAuthority.Data.Entities;

namespace PortAuthority.Data
{
    public interface IPortAuthorityDbContext 
        : IDisposable, IAsyncDisposable
    {
        DbSet<Job> Jobs { get; }
        DbSet<Subtask> Tasks { get; }
        
        DatabaseFacade Database { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
    }
}
