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
    public class StartJobConsumerTest
    {
        // class under test
        private StartJobConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new StartJobConsumer(
                loggerFactory.CreateLogger<StartJobConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_StartJob_Should_Be_InProgress_With_StartTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestStartJobMessage
            {
                JobId = job.JobId,
                StartTime = DateTimeOffset.Now
            };
            
            // act;
            await _consumer.Consume(new TestConsumeContext<StartJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.InProgress, because: "job status updated");
            actual.StartTime.Should().BeCloseTo(message.StartTime, because: "job start time updated");
        }
        
        [Test]
        public async Task Test_StartJob_NotFound_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestStartJobMessage
            {
                JobId = Guid.NewGuid(), /* job does not exist */
                StartTime = DateTimeOffset.Now.AddDays(99)
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<StartJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status);
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "job start time should not have changed");
        }        
        
        [Test]
        public async Task Test_StartJob_AlreadyInProgress_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestStartJobMessage
            {
                JobId = job.JobId,
                StartTime = DateTimeOffset.Now.AddDays(99)
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<StartJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status);
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "job start time should not have changed");
        }
    }
}
