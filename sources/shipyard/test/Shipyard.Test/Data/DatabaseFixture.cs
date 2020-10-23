using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shipyard.Bootstrap;
using Shipyard.Data;

namespace Shipyard.Test.Data
{
    /// <summary>
    /// EntityFramework database testing fixture using a SqlLite database.
    /// </summary>
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            Configuration = ConfigurationHelper.GetConfiguration();
            ServiceProvider = ServiceCollectionHelper.BuildServiceProvider(ConfigureServices);
        }
        
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging => logging
                .AddConfiguration(Configuration)
                .AddConsole());
            
            services.AddDbContext<IShipyardDbContext, ShipyardDbContext>(options => options
                .UseSqlite(Configuration.GetConnectionString("SqlDatabase")));

            services.AddShipyardServices();
        }

        /// <summary>
        /// Configuration root
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Dependency Injection service provider (i.e., the DI container)
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }
        

        /// <summary>
        /// Create an instance of the GetDbContext
        /// </summary>
        /// <returns></returns>
        protected ShipyardDbContext GetDbContext()
        {
            // Note: The DbContext will automatically dispose of the parent scope. Make sure that
            //       all usage of the db context is properly disposed of or wrapped in a 'using' statement.
            var serviceScope = ServiceProvider.CreateScope();
            return (ShipyardDbContext) serviceScope.ServiceProvider.GetRequiredService<IShipyardDbContext>();
        }
        
        /// <summary>
        /// Ensures that the database has been created before the test suite is started.
        /// </summary>
        /// <returns></returns>
        [OneTimeSetUp]
        protected async Task EnsureCreated()
        {
            using var scope = ServiceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<IShipyardDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
        
        /// <summary>
        /// Ensures that the database has been deleted after the test suite has completed.
        /// </summary>
        /// <returns></returns>
        [OneTimeTearDown]
        protected async Task EnsureDeleted()
        {
            using var scope = ServiceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<IShipyardDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }        
    }
}
