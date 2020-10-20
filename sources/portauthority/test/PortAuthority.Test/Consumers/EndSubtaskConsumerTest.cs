using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PortAuthority.Consumers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class EndSubtaskConsumerTest
    {
        // class under test
        private EndSubtaskConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new EndSubtaskConsumer(
                loggerFactory.CreateLogger<EndSubtaskConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_EndSubtask_Success_True_Should_Be_Completed_With_EndTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,InProgress");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestEndSubtaskMessage()
            {
                TaskId = task.TaskId,
                EndTime = DateTimeOffset.Now,
                Success = true
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.Completed, because: "task status updated");
            actual.EndTime.Should().BeCloseTo(message.EndTime, because: "task end time updated");
        }
        
        [Test]
        public async Task Test_EndSubtask_Success_False_Should_Be_Failed_With_EndTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,InProgress");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestEndSubtaskMessage()
            {
                TaskId = task.TaskId,
                EndTime = DateTimeOffset.Now,
                Success = false
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.Failed, because: "task status updated");
            actual.EndTime.Should().BeCloseTo(message.EndTime, because: "task end time updated");
        }
        
        [Test]
        public async Task Test_EndJob_NotFound_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,InProgress");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestEndSubtaskMessage()
            {
                TaskId = Guid.NewGuid(), /* task does not exist */
                EndTime = DateTimeOffset.Now.AddDays(99),
                Success = false
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status, "task start time should not have changed");
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "task start time should not have changed");
            actual.EndTime.Should().BeNull("task end time not set");
        }        
        
        [Test]
        public async Task Test_EndSubtask_AlreadyComplete_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,Completed");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);

            var message = new TestEndSubtaskMessage()
            {
                TaskId = task.TaskId,
                EndTime = DateTimeOffset.Now.AddDays(99),
                Success = true
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status, "task start time should not have changed");
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "task start time should not have changed");
            actual.EndTime.Should().BeNull("task end time not set");
        }
    }
}
