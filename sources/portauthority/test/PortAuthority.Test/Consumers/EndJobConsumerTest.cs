using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;
using PortAuthority.Worker.Consumer;

namespace PortAuthority.Test.Consumers
{
    public class EndJobConsumerTest
    {
        // class under test
        private EndJobConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new EndJobConsumer(
                loggerFactory.CreateLogger<EndJobConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_EndJob_Success_True_Should_Be_Completed_With_EndTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestEndJobMessage()
            {
                JobId = job.JobId,
                EndTime = DateTimeOffset.Now,
                Success = true
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.Completed, because: "job status updated");
            actual.EndTime.Should().BeCloseTo(message.EndTime, because: "job end time updated");
        }
        
        [Test]
        public async Task Test_EndJob_Success_False_Should_Be_Failed_With_EndTime()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestEndJobMessage()
            {
                JobId = job.JobId,
                EndTime = DateTimeOffset.Now,
                Success = false
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.Failed, because: "job status updated");
            actual.EndTime.Should().BeCloseTo(message.EndTime, because: "job end time updated");
        }
        
        [Test]
        public async Task Test_EndJob_NotFound_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestEndJobMessage()
            {
                JobId = Guid.NewGuid(), /* job does not exist */
                EndTime = DateTimeOffset.Now.AddDays(99),
                Success = false
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status);
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "job start time should not have changed");
            actual.EndTime.Should().BeNull("bad job id, end time not set");
        }        
        
        [Test]
        public async Task Test_EndJob_AlreadyComplete_Should_noop()
        {
            // arrange
            var job = new JobFaker().Generate("default,Completed");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestEndJobMessage()
            {
                JobId = job.JobId,
                EndTime = DateTimeOffset.Now.AddDays(99),
                Success = true
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<EndJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(job.Status);
            actual.StartTime.Should().BeCloseTo(job.StartTime.Value, because: "job start time should not have changed");
            actual.EndTime.Should().BeCloseTo(job.EndTime.Value, because: "job end time should not have changed");
        }
    }
}
