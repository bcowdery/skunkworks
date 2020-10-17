using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Data
{
    public class DbContextTest_Jobs 
        : DatabaseFixture
    {
        [Test]
        public async Task Test_AddJob_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate();

            // act
            await using var dbContext = GetDbContext();
            await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();

            // assert
            await using var actualDbContext = GetDbContext();
            var actual = actualDbContext.Jobs.Find(job.Id);
            actual.Should().BeEquivalentTo(job);
        }

        [Test]
        public async Task Test_UpdateJob_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);

            // act
            var updated = dbContext.Jobs.Find(job.Id);
            updated.Status = Status.InProgress;
            updated.StartTime = DateTimeOffset.Now;

            await dbContext.SaveChangesAsync();

            // assert
            await using var actualDbContext = GetDbContext();
            var actual = actualDbContext.Jobs.Find(job.Id);
            actual.Should().BeEquivalentTo(updated);
        }
        
        [Test]
        public async Task Test_UpdateJob_AddTask_Should_Persist()
        {
            // arrange
            var job = new JobFaker().Generate();
            var tasks = new SubtaskFaker().Generate(10);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);

            // act
            var entity = dbContext.Jobs.Find(job.Id);
            entity.Tasks.AddRange(tasks);
            await dbContext.SaveChangesAsync();
        
            // assert
            await using var actualDbContext = GetDbContext();
            var actual = actualDbContext.Jobs
                .Include(x => x.Tasks)
                .Single(x => x.Id == job.Id);

            actual.Tasks.Should().HaveCount(10);
        
        }

        [Test]
        public async Task Test_FindJob_ByGuid_Should_Match()
        {
            // arrange
            var jobs = new JobFaker().Generate(10);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, jobs);

            // act
            var expected = jobs[3]; /* random selection by fair dice roll */
            var actual = dbContext.Jobs.FirstOrDefault(x => x.JobId == expected.JobId);

            // assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

