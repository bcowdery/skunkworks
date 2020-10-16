using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PortAuthority.Test.Mocks
{
    public static class DbContextTestExtensions
    {
        /// <summary>
        /// Setup test data by adding it to the GetDbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="selector"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        public static void Setup<TContext, T>(this TContext dbContext, Func<TContext, DbSet<T>> selector, IEnumerable<T> entities)
            where TContext : DbContext
            where T : class
        {
            var dbSet = selector(dbContext);
            dbSet.AddRange(entities);
            dbContext.SaveChanges();
        }

        /*
        /// <summary>
        /// Verify the state of the GetDbContext, asserting that an element exists matching the search predicate.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <exception cref="AssertionException"></exception>
        public static void Verify<TContext, T>(this TContext dbContext, Func<TContext, DbSet<T>> selector, Expression<Func<T, bool>> predicate)
            where TContext : DbContext
            where T : class
        {
            var dbSet = selector(dbContext);
            if (!dbSet.Any(predicate))
            {
                throw new AssertionException($"Element not found matching \"{predicate.Body}\"");
            }
        }

        /// <summary>
        /// Verify that only one element in the GetDbContext matches the search predicate.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <exception cref="AssertionException"></exception>
        public static void VerifyOne<TContext, T>(this TContext dbContext, Func<TContext, DbSet<T>> selector, Expression<Func<T, bool>> predicate)
            where TContext : DbContext
            where T : class
        {
            var dbSet = selector(dbContext);

            try
            {
                var match = dbSet.SingleOrDefault(predicate);
                if (match == null)
                {
                    throw new AssertionException($"Element not found matching \"{predicate.Body}\"");
                }
            }
            catch (InvalidOperationException)
            {
                throw new AssertionException($"More than one element found matching \"{predicate.Body}\"");
            }
        }       
        */ 
    }
}
