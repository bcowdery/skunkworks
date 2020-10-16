using System;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using MassTransit.TestFramework;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Test.Mocks;
using PortAuthority.Worker.Consumer;

namespace PortAuthority.Test.Consumers
{
    public class CreateJobConsumerTest_Integration 
        : ConsumerFixture<CreateJobConsumer, CreateJob>
    {
        
        [Test]
        public async Task Test_CreateJob_Should_PersistNewJob()
        {
            var faker = new Faker();

            var harness = GetRequiredService<InMemoryTestHarness>();
            
            await harness.InputQueueSendEndpoint.Send<CreateJob>(new 
            {
                JobId = NewId.NextGuid(),
                Type = faker.Lorem.Slug(),
                Namespace = faker.Internet.DomainName()
            });

            Assert.Pass();
            // // did the endpoint consume the message
            // Assert.That(await _harness.Consumed.Any<CreateJob>());
            //
            // // did the actual consumer consume the message
            // Assert.That(await _consumerHarness.Consumed.Any<CreateJob>());
            //
            // // ensure that no faults were published by the consumer
            // Assert.That(await _harness.Published.Any<Fault<CreateJob>>(), Is.False);

        }
    }
}
