using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Mocks;
using PortAuthority.Worker.Consumer;

namespace PortAuthority.Test.Consumers
{
    public class StartJobConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<StartJobConsumer>();
        }

        [Test]
        public async Task Test_StartJobConsumer_Should_Consume_And_Status_InProgress()
        {
            // arrange
            var consumerHarness = Consumer<StartJobConsumer>();
            
            var job = new JobFaker().Generate("default,Pending");

            await using var dbContext = GetDbContext();
            dbContext.Setup(x => x.Jobs, new [] { job });
            
            var message = new TestStartJobMessage()
            {
                JobId = job.JobId,
                StartTime = DateTimeOffset.Now
            };
            
            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<StartJob>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<StartJob>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<StartJob>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<StartJob>>(), Is.False, "message handled without fault");

                await using var actualDbContext = GetDbContext();
                var actual = actualDbContext.Jobs.SingleOrDefault(j => j.JobId == message.JobId);
                
                actual.Should().NotBeNull();
                actual.JobId.Should().Be(message.JobId);
                actual.Status.Should().Be(Status.InProgress);
                actual.StartTime.Should().BeCloseTo(message.StartTime);
            }
            finally
            {
                await Harness.Stop();
            }
        }    
    }
}
