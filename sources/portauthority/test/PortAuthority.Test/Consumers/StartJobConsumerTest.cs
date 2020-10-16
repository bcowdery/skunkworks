using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
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
            var faker = new Faker();
            var job = new JobFaker().Generate("default,Pending");

            await using var dbContext = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await dbContext.Setup(x => x.Jobs, new [] { job });
            
            var message = new TestStartJobMessage
            {
                JobId = job.JobId,
                StartTime = faker.Date.RecentOffset()
            };
            
            // act
            var consumeContext = new TestConsumeContext<StartJob>(message);
            await _consumer.Consume(consumeContext);

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == job.JobId);
            
            actual.Should().NotBeNull();
            actual.Status.Should().Be(Status.InProgress);
            actual.StartTime.Should().BeCloseTo(message.StartTime);
        }
    }
}
