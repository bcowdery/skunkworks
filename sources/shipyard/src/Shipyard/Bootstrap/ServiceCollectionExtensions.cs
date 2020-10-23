using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shipyard.Assemblers;
using Shipyard.Contracts;
using Shipyard.Contracts.MessageTypes;
using Shipyard.Providers;
using Shipyard.Providers.SendGrid;
using Shipyard.Providers.Twilio;

namespace Shipyard.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register required services for the Shipyard system.
        /// </summary>
        /// <param name="services"></param>
        public static void AddShipyardServices(this IServiceCollection services) 
        {
            // Register services by convention
            services.Scan(scan => scan
                .FromAssemblies(AssemblyHook.Assembly)

                // Assemblers
                .AddClasses(c => c.AssignableTo(typeof(IAssembler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                // Validators
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            
            // Message providers
            services.AddTransient<IMessageProvider<IEmail>, SendGridMessageProvider>();
            services.AddTransient<IMessageProvider<ISms>, TwilioMessageProvider>();

            // Declared services 
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
