using System;
using Microsoft.Extensions.DependencyInjection;

namespace PortAuthority.Test
{
    /// <summary>
    /// Helper to build Microsoft DependencyInjection service providers.
    /// </summary>
    public static class ServiceCollectionHelper
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> configure)
        {
            var serviceCollection = new ServiceCollection();
            configure(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
    }
}
