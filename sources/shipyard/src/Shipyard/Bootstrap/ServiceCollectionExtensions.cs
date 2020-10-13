using Microsoft.Extensions.DependencyInjection;
using Shipyard.Contracts;
using Shipyard.Providers;
using Shipyard.Providers.SendGrid;
using Shipyard.Providers.Twilio;

namespace Shipyard.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterShipyardServices(this IServiceCollection services) 
        {
            // TODO: Assert that required services (e.g., DbOptions<ShipyardDbContext>) are registered so you can't fuck it up.

            // Message providers
            services.AddTransient<IMessageProvider<IEmail>, SendGridMessageProvider>();
            services.AddTransient<IMessageProvider<ISms>, TwilioMessageProvider>();

            // Services
            services.AddTransient<EmailService>();
        }
    }
}
