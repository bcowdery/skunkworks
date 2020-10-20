using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortAuthority.Bootstrap;
using PortAuthority.Data;
using PortAuthority.Consumers;

namespace PortAuthority.Worker
{
    public class Program
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
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddApplicationInsights();
                    logging.AddConsole();
                })                
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    // EF Databases
                    services.AddDbContext<IPortAuthorityDbContext, PortAuthorityDbContext>(options => options
                        .UseSqlServer(configuration.GetConnectionString("SqlDatabase"), 
                            providerOptions => providerOptions.EnableRetryOnFailure()));

                    // Mass Transit
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumersFromNamespaceContaining<CreateJobConsumer>();
                        x.SetKebabCaseEndpointNameFormatter();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.AmqpHost(configuration.GetConnectionString("Rabbit"));
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                    
                    services.AddMassTransitHostedService();
                    
                    // Application Services
                    services.AddPortAuthorityServices();
                    services.AddHostedService<WorkerBackgroundService>();
                })
                .UseConsoleLifetime();                
    }
}
