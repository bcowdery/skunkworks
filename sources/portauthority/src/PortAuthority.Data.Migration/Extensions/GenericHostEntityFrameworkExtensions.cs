using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PortAuthority.Data.Migration.Extensions
{
    public static class GenericHostEntityFrameworkExtensions
    {
        /// <summary>
        /// Allows EntityFramework migrations to be run from the generic host on startup.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task<IHost> MigrateDatabaseAsync<T>(this IHost host) 
            where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                await using var dbContext = services.GetRequiredService<T>();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }

            return host;
        }
    }
}
