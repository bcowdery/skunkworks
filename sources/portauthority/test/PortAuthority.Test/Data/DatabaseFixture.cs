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
        
        /// <summary>Lifetime scope of the currently running test.</summary>
        private IServiceScope _testScope;


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
        /// Disposes of the current test lifetime scope.
        /// </summary>
        [TearDown]
        protected void DisposeTestScope()
        {
            _testScope?.Dispose();
            _testScope = null;
        }
        
        /// <summary>
        /// Create an instance of the DbContext
        /// </summary>
        /// <returns></returns>
        protected IPortAuthorityDbContext CreateDbContext()
        {
            _testScope ??= ServiceProvider.CreateScope();
            return _testScope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();
        }

        /// <summary>
        /// Perform an action in a new lifetime scope.
        /// </summary>
        /// <param name="action"></param>
        protected void Scoped(Action<IPortAuthorityDbContext> action)
        {
            using var scope = ServiceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();

            action(dbContext);
        }

        /// <summary>
        /// Perform an action in a new lifetime scope and return the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        protected T Scoped<T>(Func<IPortAuthorityDbContext, T> action)
        {
            using var scope = ServiceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();

            return action(dbContext);
        }

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
