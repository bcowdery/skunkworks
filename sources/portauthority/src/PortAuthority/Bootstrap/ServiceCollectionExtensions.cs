using Microsoft.Extensions.DependencyInjection;
using PortAuthority.Contracts;

namespace PortAuthority.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterPortAuthorityServices(this IServiceCollection services) 
        {
            // TODO: Assert that required services (e.g., DbOptions<PortAuthorityDbContext>) are registered so you can't fuck it up.

            // Services            
        }
    }
}
