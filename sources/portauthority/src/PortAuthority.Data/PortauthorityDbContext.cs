using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PortAuthority.Data.Migrations.Internal;

namespace PortAuthority.Data
{
    public class PortAuthorityDbContext : DbContext
    {
        public PortAuthorityDbContext(DbContextOptions<PortAuthorityDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // must be first
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IHistoryRepository, PortAuthorityMigrationHistoryRepository>();
        }
    }
}
