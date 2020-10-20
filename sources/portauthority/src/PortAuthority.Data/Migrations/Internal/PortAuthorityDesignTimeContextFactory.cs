using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PortAuthority.Data.Migrations.Internal
{
    public class PortAuthorityDesignTimeContextFactory
        : IDesignTimeDbContextFactory<PortAuthorityDbContext>
    {
        public PortAuthorityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PortAuthorityDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,11433;Database=skunkworks;User=sa;Password=r00tp@ssw0rD");
            
            return new PortAuthorityDbContext(optionsBuilder.Options);
        }
    }
}
