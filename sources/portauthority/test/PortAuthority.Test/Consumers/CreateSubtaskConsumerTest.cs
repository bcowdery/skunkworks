using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PortAuthority.Consumers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class CreateSubtaskConsumerTest
    {
        // class under test
        private CreateSubtaskConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new CreateSubtaskConsumer(
                loggerFactory.CreateLogger<CreateSubtaskConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_CreateSubtask_Should_PersistNewJob()
        {
            // arrange
            var job = new JobFaker().Generate();
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestCreateSubtaskMessage()
            {
                JobId = job.JobId,
                TaskId = NewId.NextGuid(),
                Name = "test-create-subtask"
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<CreateSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.SingleOrDefault(t => t.TaskId == message.TaskId);

            actual.Should().NotBeNull();
            actual.TaskId.Should().Be(message.TaskId);
            actual.Name.Should().Be(message.Name);
        }
        
        [Test]
        public async Task Test_CreateSubtask_Invalid_ParentJobId_Should_Throw_InvalidOperationException()
        {
            // arrange
            var job = new JobFaker().Generate();
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestCreateSubtaskMessage()
            {
                JobId = Guid.NewGuid(),
                TaskId = NewId.NextGuid(),
                Name = "test-create-subtask-invalid-jobid"
            };
            
            // act
            Func<Task> act = async () => { await _consumer.Consume(new TestConsumeContext<CreateSubtask>(message)); };
            
            // assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }    
        
        [Test]
        public async Task Test_CreateSubtask_Empty_TaskId_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate();
            
            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestCreateSubtaskMessage()
            {
                JobId = job.JobId,
                TaskId = Guid.Empty,
                Name = "test-create-subtask-empty-taskid"
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<CreateSubtask>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Tasks.ToList();

            actual.Should().BeNullOrEmpty("no tasks created without a valid parent job id");
        }          
    }
}
