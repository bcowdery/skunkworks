using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PortAuthority.Bootstrap;
using PortAuthority.Data;

namespace PortAuthority.Test.Consumers
{
    /// <summary>
    /// MassTransit consumer testing fixture using an in-memory service bus transport.
    /// </summary>
    public class ConsumerFixture
    {
        public ConsumerFixture()
        {
            Configuration = ConfigurationHelper.GetConfiguration();
            ServiceProvider = ServiceCollectionHelper.BuildServiceProvider(ConfigureServices);
            Harness = ServiceProvider.GetRequiredService<InMemoryTestHarness>();
        }
        
        
        /// <summary>
        /// Add services to the container
        /// </summary>
        /// <param name="services"></param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging => logging
                .AddConfiguration(Configuration)
                .AddConsole());
            
            services.AddDbContext<IPortAuthorityDbContext, PortAuthorityDbContext>(options => options
                .UseSqlite(Configuration.GetConnectionString("Default")));

            services.AddPortAuthorityServices();
            services.AddMassTransitInMemoryTestHarness(ConfigureBus);
        }

        /// <summary>
        /// Add service bus configuration
        /// </summary>
        /// <param name="bus"></param>
        protected virtual void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
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
        /// MassTransit in-memory testing harness.
        /// </summary>
        protected InMemoryTestHarness Harness { get; }        
        
        /// <summary>
        /// Constructs an <see cref="IConsumerTestHarness{TConsumer}"/> for testing MassTransit consumers in
        /// an integration testing scenario. Allows for validation of sent and consumed messages.
        /// </summary>        
        protected IConsumerTestHarness<TConsumer> Consumer<TConsumer>()
            where TConsumer: class, IConsumer
        {
            return Harness.Consumer(() => ServiceProvider.GetRequiredService<TConsumer>());
        }

        /// <summary>
        /// Create an instance of the GetDbContext. DbContext MUST BE DISPOSED OF AFTER USE.
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
