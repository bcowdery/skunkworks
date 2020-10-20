using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit.TestFramework;
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
    public class StartSubtaskConsumerTest
    {
        // class under test
        private StartSubtaskConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new StartSubtaskConsumer(
                loggerFactory.CreateLogger<StartSubtaskConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_StartSubtask_Should_Be_InProgress_With_StartTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,Pending");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestStartSubtaskMessage
            {
                TaskId = task.TaskId,
                StartTime = DateTimeOffset.Now
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<StartSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.InProgress, because: "sub-task status updated");
            actual.StartTime.Should().BeCloseTo(message.StartTime, because: "sub-task start time updated");
        }
        
        [Test]
        public async Task Test_StartSubtask_NotFound_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,Pending");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestStartSubtaskMessage
            {
                TaskId = Guid.NewGuid(), /* task does not exist */
                StartTime = DateTimeOffset.Now
            };

            // act
            await _consumer.Consume(new TestConsumeContext<StartSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(task.Status, "task status should not have changed");
            actual.StartTime.Should().Be(task.StartTime, because: "task start time should not have changed");
        }        
        
        [Test]
        public async Task Test_StartJob_AlreadyInProgress_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("defaults,InProgress");
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);

            var message = new TestStartSubtaskMessage
            {
                TaskId = task.TaskId,
                StartTime = DateTimeOffset.Now.AddDays(99)
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<StartSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(task.Status, "task status should not have changed");
            actual.StartTime.Should().BeCloseTo(task.StartTime.Value, because: "task start time should not have changed");
        }
    }
}
