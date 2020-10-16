using System;
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
    /// Database fixture for working with the EF DbContext using a SqlLite database.
    /// </summary>
    public class DatabaseFixture
    {
        private IServiceScope _testScope;


        public DatabaseFixture()
        {
            Configuration = ConfigurationHelper.GetConfiguration();
            Services = ServiceCollectionHelper.BuildServiceProvider(ConfigureServices);
        }

        
        /// <summary>
        /// Configuration root
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Dependency Injection service provider (i.e., the DI container)
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Logger factory for console output
        /// </summary>
        protected ILoggerFactory LogFactory => LoggerFactory.Create(builder => builder
            .AddConfiguration(Configuration)
            .AddConsole());

        protected void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddDbContext<IPortAuthorityDbContext, PortAuthorityDbContext>(options => options
                .UseSqlite(Configuration.GetConnectionString("Default"))
                .UseLoggerFactory(LogFactory)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging());

            services.AddPortAuthorityServices();
        }

        [SetUp]
        protected void SetupTestScope()
        {
            _testScope = Services.CreateScope();
        }

        [TearDown]
        protected void TearDownTestScope()
        {
            _testScope?.Dispose();
            _testScope = null;
        }
        
        /// <summary>
        /// Create an instance of the DbContext
        /// </summary>
        /// <returns></returns>
        public IPortAuthorityDbContext CreateDbContext()
        {
            return _testScope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();
        }

        /// <summary>
        /// Perform an action in a new lifetime scope.
        /// </summary>
        /// <param name="action"></param>
        public void Scoped(Action<IPortAuthorityDbContext> action)
        {
            using var scope = Services.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();

            action(dbContext);
        }

        /// <summary>
        /// Perform an action in a new lifetime scope and return the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public T Scoped<T>(Func<IPortAuthorityDbContext, T> action)
        {
            using var scope = Services.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<IPortAuthorityDbContext>();

            return action(dbContext);
        }
        
        /// <summary>
        /// Bring the database up-to-date using migrations.
        /// </summary>
        public void Migrate()
        {
            Scoped(ctx =>
            {
                ctx.Database.Migrate();
            });
        }

        /// <summary>
        /// Create the database
        /// </summary>
        [OneTimeSetUp]
        public void EnsureCreated()
        {
            Scoped(ctx =>
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            });
        }
        
        /// <summary>
        /// Delete the database
        /// </summary>
        [OneTimeTearDown]
        public void EnsureDeleted()
        {
            Scoped(ctx =>
            {
                ctx.Database.EnsureDeleted();
            });
        }
    }
}
