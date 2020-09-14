using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Shipyard.Data.Migrations.Internal;

namespace Shipyard.Data
{
    public class ShipyardDbContext : DbContext
    {
        public ShipyardDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // must be first
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IHistoryRepository, ShipyardMigrationHistoryRepository>();
        }
    }
}
