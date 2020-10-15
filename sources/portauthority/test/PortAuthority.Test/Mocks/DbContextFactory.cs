using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PortAuthority.Data;

namespace PortAuthority.Test.Mocks
{
    public class DbContextFactory
    {
        public DbContextFactory()
        {
        }

        /// <summary>
        /// Returns the shared instance of <see cref="DbContextFactory"/>.
        /// </summary>
        public static readonly DbContextFactory Instance = new DbContextFactory();
        
        /// <summary>
        /// Creates a new in-memory DbContext for testing.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public TDbContext CreateDbContext<TDbContext>()
            where TDbContext : DbContext
        {
            var dbName = TestContext.CurrentContext.Test.Name;
            var dbOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return (TDbContext) Activator.CreateInstance(typeof(TDbContext), dbOptions);
        }
    }
}
