using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace PortAuthority.Test.Mocks
{
    /// <summary>
    /// Wrapper for DbContext testing that provides some easy to use setup methods.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class DbContextTestWrapper<TContext> : IDisposable
        where TContext : DbContext
    {

        private readonly TContext _dbContext;
        
        public DbContextTestWrapper(TContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Setup test data by adding it to the DbContext.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        public void Setup<T>(Func<TContext, DbSet<T>> selector, IEnumerable<T> entities)
            where T : class
        {
            var dbSet = selector(_dbContext);
            dbSet.AddRange(entities);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Verify the state of the DbContext, asserting that an element exists matching the search predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="AssertionException"></exception>
        public void Verify<T>(Func<TContext, DbSet<T>> selector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            var dbSet = selector(_dbContext);
            if (!dbSet.Any(predicate))
            {
                throw new AssertionException($"Element not found matching \"{predicate.Body}\"");
            }
        }

        /// <summary>
        /// Verify that only one element in the DbContext matches the search predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="AssertionException"></exception>
        public void VerifyOne<T>(Func<TContext, DbSet<T>> selector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            var dbSet = selector(_dbContext);

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
        
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
    
    /// <summary>
    /// Extension methods for producing testable <see cref="DbContext"/> instances.
    /// </summary>
    public static class DbContextTestWrapperExtensions 
    {
        /// <summary>
        /// Wraps a DbContext with the <seealso cref="DbContextTestWrapper{TContext}"/>
        /// </summary>
        /// <param name="dbContext"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextTestWrapper<TContext> AsTestable<TContext>(this TContext dbContext)
            where TContext : DbContext
        {
            return new DbContextTestWrapper<TContext>(dbContext);
        }

        /// <summary>
        /// Creates a new DbContext and wraps it with the <seealso cref="DbContextTestWrapper{TContext}"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static DbContextTestWrapper<TContext> CreateTestable<TContext>(this DbContextFactory factory)
            where TContext : DbContext
        {
            var dbContext = factory.CreateDbContext<TContext>();
            return new DbContextTestWrapper<TContext>(dbContext);
        }
    }
}
