using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Fakes;

namespace PortAuthority.Test.Data
{
    public class DbContextTest_Jobs 
        : DatabaseFixture
    {
        [Test]
        public void Test_AddJob_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate();

            // act
            using var dbContext = CreateDbContext();
            dbContext.Jobs.Add(job);
            dbContext.SaveChanges();

            // assert
            Scoped(ctx =>
            {
                var actual = ctx.Jobs.Find(job.Id);
                actual.Should().BeEquivalentTo(job);
            });
        }

        [Test]
        public void Test_UpdateJob_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");

            using var dbContext = CreateDbContext();
            dbContext.Jobs.Add(job);
            dbContext.SaveChanges();

            // act
            var updated = Scoped(ctx =>
            {
                var entity = ctx.Jobs.Find(job.Id);
                entity.Status = Status.InProgress;
                entity.StartTime = DateTimeOffset.Now;

                ctx.SaveChanges();

                return entity;
            });

            // assert
            Scoped(ctx =>
            {
                var actual = ctx.Jobs.Find(job.Id);
                actual.Should().BeEquivalentTo(updated);
            });
        }
        
        [Test]
        public void Test_UpdateJob_AddTask_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate();
            
            using var dbContext = CreateDbContext();
            dbContext.Jobs.Add(job);
            dbContext.SaveChanges();

            var tasks = new SubtaskFaker().Generate(10);

            // act
            Scoped(ctx =>
            {
                var entity = ctx.Jobs.Find(job.Id);
                entity.Tasks.AddRange(tasks);

                ctx.SaveChanges();
            });

            // assert
            Scoped(ctx =>
            {
                var actual = ctx.Jobs
                    .Include(x => x.Tasks)
                    .Single(x => x.Id == job.Id);

                actual.Tasks.Should().HaveCount(10);
            });
        }

        [Test]
        public void Test_FindJob_ByGuid_Should_Match()
        {
            // arrange
            var jobs = new JobFaker().Generate(10);
            
            using var dbContext = CreateDbContext();
            dbContext.Jobs.AddRange(jobs);
            dbContext.SaveChanges();

            // act
            var expected = jobs[3]; /* random selection by fair dice roll */
            var actual = dbContext.Jobs.FirstOrDefault(x => x.JobId == expected.JobId);

            // assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

