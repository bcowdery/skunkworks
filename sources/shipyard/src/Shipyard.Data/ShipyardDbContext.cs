using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Shipyard.Data.Migrations.Internal;

namespace Shipyard.Data
{
    public class ShipyardDbContext 
        : DbContext, IShipyardDbContext
    {
        public ShipyardDbContext(DbContextOptions<ShipyardDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // must be first

            modelBuilder.HasDefaultSchema("yrd");
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyHook.Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) 
            optionsBuilder.ReplaceService<IHistoryRepository, ShipyardMigrationHistoryRepository>();
        }
    }
}
