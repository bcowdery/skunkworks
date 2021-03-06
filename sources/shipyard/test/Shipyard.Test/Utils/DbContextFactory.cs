﻿using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Shipyard.Test.Utils
{
    /// <summary>
    /// Factory for producing in-memory instances of the EntityFramework <see cref="DbContext"/>.
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// Returns the shared instance of <see cref="DbContextFactory"/>.
        /// </summary>
        public static readonly DbContextFactory Instance = new DbContextFactory();
        
        private DbContextFactory()
        {
        }

        /// <summary>
        /// Creates a new in-memory GetDbContext for testing.
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
