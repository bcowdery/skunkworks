using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shipyard.App.Bootstrap;
using Shipyard.Data;

namespace Shipyard.App
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Splash.Print(Console.Write);

            using var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.AddLogging();
                    services.AddHostedService<ShipyardService>();
                    services.AddDbContext<ShipyardDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("Default"), 
                            providerOptions => providerOptions.EnableRetryOnFailure()));
                    
                })
                .ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                })
                .UseConsoleLifetime();
    }
}
