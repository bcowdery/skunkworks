using System;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Test.Mocks;
using PortAuthority.Worker.Consumer;

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
            var faker = new Faker();
            var message = new TestCreateJobMessage
            {
                JobId = NewId.NextGuid(),
                Type = faker.Lorem.Slug(),
                Namespace = faker.Internet.DomainName()
            };
            
            // act
            var consumeContext = new TestConsumeContext<CreateJob>(message);
            await _consumer.Consume(consumeContext);

            // assert
            using var dbContext = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            dbContext.VerifyOne(x => x.Jobs, j => j.JobId == message.JobId);
        }
    }

    /// <summary>
    /// Test implementation of <see cref="CreateJob"/>.
    /// </summary>
    internal sealed class TestCreateJobMessage : CreateJob
    {
        public Guid JobId { get; set; }
        public Guid? CorrelationId { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
    }
}
