using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PortAuthority.Bootstrap;
using PortAuthority.Data;

namespace PortAuthority.Test.Data
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
            
            services.AddDbContext<IPortAuthorityDbContext, PortAuthorityDbContext>(options => options
                .UseSqlite(Configuration.GetConnectionString("Default")));

            services.AddPortAuthorityServices();
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
        protected PortAuthorityDbContext GetDbContext()
        {
            // Note: The DbContext will automatically dispose of the parent scope. Make sure that
            //       all usage of the db context is properly disposed of or wrapped in a 'using' statement.
            var serviceScope = ServiceProvider.CreateScope();
            return (PortAuthorityDbContext) serviceScope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();
        }
        
        /// <summary>
        /// Get service of type T from the test service provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() => ServiceProvider.GetService<T>();

        /// <summary>
        /// Get service of type T from the test service provider. Throws an exception if not found. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// Ensures that the database has been created before the test suite is started.
        /// </summary>
        /// <returns></returns>
        [OneTimeSetUp]
        protected async Task EnsureCreated()
        {
            using var scope = ServiceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();
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
            await using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
        }        
    }
}
