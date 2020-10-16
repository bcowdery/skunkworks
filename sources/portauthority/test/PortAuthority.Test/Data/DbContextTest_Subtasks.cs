using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.JobService.Components.StateMachines;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Mocks;

namespace PortAuthority.Test.Data
{
    public class DbContextTest_Subtasks 
        : DatabaseFixture
    {

        // test data
        private Job _job;

        
        [SetUp]
        public async Task SetupJob()
        {
            _job = new JobFaker().Generate("default,Pending");

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, _job);
        }
        

        [Test]
        public async Task Test_AddTask_Should_Persist()
        {
            // arrange
            var task = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate();

            // act
            await using var dbContext = GetDbContext();
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // assert
            await using var actualDbContext = GetDbContext();
            var actual = actualDbContext.Tasks.Find(task.Id);
            actual.Should().BeEquivalentTo(task);
        }

        [Test]
        public async Task Test_UpdateTask_Should_Persist()
        {
            // arrange
            var task = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate("default,Pending");

            await using var dbContext = GetDbContext();
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // act
            var updated = dbContext.Tasks.Find(task.Id);
            updated.Status = Status.InProgress;
            updated.StartTime = DateTimeOffset.Now;

            await dbContext.SaveChangesAsync();

            // assert
            await using var actualDbContext = GetDbContext();
            var actual = actualDbContext.Tasks.Find(task.Id);
            actual.Should().BeEquivalentTo(updated);
        }
        
        [Test]
        public async Task Test_FindTask_ByGuid_Should_Match()
        {
            // arrange
            var tasks = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate(10);
            
            await using var dbContext = GetDbContext();
            await dbContext.Tasks.AddRangeAsync(tasks);
            await dbContext.SaveChangesAsync();

            // act
            var expected = tasks[7]; /* random selection by fair dice roll */
            var actual = dbContext.Tasks.FirstOrDefault(x => x.TaskId == expected.TaskId);

            // assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

