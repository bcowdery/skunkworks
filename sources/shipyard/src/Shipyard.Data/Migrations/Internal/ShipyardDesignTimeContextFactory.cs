using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shipyard.Data.Migrations.Internal
{
    public class ShipyardDesignTimeContextFactory
        : IDesignTimeDbContextFactory<ShipyardDbContext>
    {
        public ShipyardDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShipyardDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,11433;Database=skunkworks;User=sa;Password=r00tp@ssw0rD");
            
            return new ShipyardDbContext(optionsBuilder.Options);
        }
    }
}
