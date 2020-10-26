using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortAuthority.Data.Migration.Extensions;

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
            using var host = CreateHostBuilder(args).Build();
            await host.MigrateDatabaseAsync<PortAuthorityDbContext>();
            await host.RunAsync(); /* does nothing, keeps the container alive */
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddApplicationInsights();
                    logging.AddConsole();
                })                
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.AddDbContext<PortAuthorityDbContext>(options => options
                        .UseSqlServer(configuration.GetConnectionString("SqlDatabase"), 
                            providerOptions => providerOptions.EnableRetryOnFailure()));
                })
                .UseConsoleLifetime();
    }
}
