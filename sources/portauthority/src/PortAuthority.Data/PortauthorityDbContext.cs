using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PortAuthority.Data.Entities;
using PortAuthority.Data.Migrations.Internal;

namespace PortAuthority.Data
{
    public class PortAuthorityDbContext : DbContext, IPortAuthorityDbContext
    {
        public PortAuthorityDbContext(DbContextOptions<PortAuthorityDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Subtask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // must be first
            
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyHook.Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) 
            optionsBuilder.ReplaceService<IHistoryRepository, PortAuthorityMigrationHistoryRepository>();
        }
    }
}
