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
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class CreateJobConsumerTest
    {
        // class under test
        private CreateJobConsumer _consumer;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _consumer = new CreateJobConsumer(
                loggerFactory.CreateLogger<CreateJobConsumer>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>()
            );
        }

        [Test]
        public async Task Test_CreateJob_Should_PersistNewJob()
        {
            // arrange
            var message = new TestCreateJobMessage
            {
                JobId = NewId.NextGuid(),
                Type = "test-createjob",
                Namespace = "com.portauthority"
            };
            
            // act
            await _consumer.Consume(new TestConsumeContext<CreateJob>(message));

            // assert
            var actual = DbContextFactory.Instance
                .CreateDbContext<PortAuthorityDbContext>()
                .Jobs.SingleOrDefault(j => j.JobId == message.JobId);

            actual.Should().NotBeNull();
            actual.JobId.Should().Be(message.JobId);
            actual.Type.Should().Be(message.Type);
            actual.Namespace.Should().Be(message.Namespace);
        }
    }
}
