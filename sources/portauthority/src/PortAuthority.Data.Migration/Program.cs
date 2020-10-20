using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PortAuthority.Data.Migration
{
    /// <summary>
    /// Runs EF Migrations against the configured database.
    /// 
    /// Uses appsettings.local.json when developing locally. Environment variables will be used as
    /// the connection string source when the migration host is executed in the compiled docker container.
    ///
    /// You may also provide connection strings as command line arguments in the format of --ConnectionStrings__SqlDatabase=Value
    /// </summary>
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PortAuthorityDbContext>()
                .UseSqlServer(configuration.GetConnectionString("SqlDatabase"), options => options
                    .EnableRetryOnFailure());

            await using var ctx = new PortAuthorityDbContext(optionsBuilder.Options);
            await ctx.Database.MigrateAsync();
        }
    }
}
