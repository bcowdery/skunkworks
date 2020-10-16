using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PortAuthority.Test.Utils
{
    public static class DbContextTestExtensions
    {
        /// <summary>
        /// Setup test data by adding it to the DbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="selector"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        public static async Task Setup<TContext, T>(this TContext dbContext, Func<TContext, DbSet<T>> selector, IEnumerable<T> entities)
            where TContext : DbContext
            where T : class
        {
            var dbSet = selector(dbContext);
            await dbSet.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Setup test data by adding it to the DbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="selector"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        public static async Task Setup<TContext, T>(this TContext dbContext, Func<TContext, DbSet<T>> selector, T entity)
            where TContext : DbContext
            where T : class
        {
            var dbSet = selector(dbContext);
            await dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
