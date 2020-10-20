using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using NUnit.Framework;
using PortAuthority.Consumers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Test.Consumers.TestMessages;

namespace PortAuthority.Test.Consumers
{
    public class CreateJobConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<CreateJobConsumer>();
        }

        [Test]
        public async Task Test_CreateJobConsumer_Should_ConsumeMessage_And_CreateJob()
        {
            // arrange
            var consumerHarness = Consumer<CreateJobConsumer>();
            
            var message = new TestCreateJobMessage()
            {
                JobId = NewId.NextGuid(),
                Type = "test-createjob",
                Namespace = "com.portauthority",
                Meta = new Dictionary<string, object>()
                {
                    { "foo", "bar"},
                    { "baz", 1 },
                    { "zap", DateTime.UtcNow }
                }                
            };
            
            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<CreateJob>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<CreateJob>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<CreateJob>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<CreateJob>>(), Is.False, "message handled without fault");

                var actual = GetDbContext().Jobs.SingleOrDefault(j => j.JobId == message.JobId);
                
                actual.Should().NotBeNull();
                actual.JobId.Should().Be(message.JobId);
                actual.Type.Should().Be(message.Type);
                actual.Namespace.Should().Be(message.Namespace);
                actual.Meta.Should().BeEquivalentTo(message.Meta);
            }
            finally
            {
                await Harness.Stop();
            }
        }    
    }
}
