using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MassTransit;
using MassTransit.JobService.Components.StateMachines;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Fakes;

namespace PortAuthority.Test.Data
{
    public class DbContextTest_Subtasks 
        : DatabaseFixture
    {

        // test data
        private Job _job;

        [SetUp]
        public void SetupJob()
        {
            _job = new JobFaker().Generate("default,Pending");

            Scoped(ctx =>
            {
                ctx.Jobs.Add(_job);
                ctx.SaveChanges();
            });
        }
        

        [Test]
        public void Test_AddTask_Should_Persist()
        {
            // arrange
            var task = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate();

            // act
            using var dbContext = CreateDbContext();
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            // assert
            Scoped(ctx =>
            {
                var actual = ctx.Tasks.Find(task.Id);
                actual.Should().BeEquivalentTo(task);
            });
        }

        [Test]
        public void Test_UpdateTask_Should_Persist()
        {
            // arrange
            var task = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate("default,Pending");

            using var dbContext = CreateDbContext();
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            // act
            var updated = Scoped(ctx =>
            {
                var entity = ctx.Tasks.Find(task.Id);
                entity.Status = Status.InProgress;
                entity.StartTime = DateTimeOffset.Now;

                ctx.SaveChanges();

                return entity;
            });

            // assert
            Scoped(ctx =>
            {
                var actual = ctx.Tasks.Find(task.Id);
                actual.Should().BeEquivalentTo(updated);
            });
        }
        
        [Test]
        public void Test_FindTask_ByGuid_Should_Match()
        {
            // arrange
            var tasks = new SubtaskFaker()
                .SetJobId(_job.Id)
                .Generate(10);
            
            using var dbContext = CreateDbContext();
            dbContext.Tasks.AddRange(tasks);
            dbContext.SaveChanges();

            // act
            var expected = tasks[7]; /* random selection by fair dice roll */
            var actual = dbContext.Tasks.FirstOrDefault(x => x.TaskId == expected.TaskId);

            // assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

