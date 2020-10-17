using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Testing;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;
using PortAuthority.Worker.Consumer;

namespace PortAuthority.Test.Consumers
{
    public class EndJobConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<EndJobConsumer>();
        }
        
        [Test]
        public async Task Test_EndJobConsumer_Success_True_Should_Consume_With_Status_Completed()
        {
            // arrange
            var consumerHarness = Consumer<EndJobConsumer>();

            var job = new JobFaker().Generate("default,InProgress");

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestEndJobMessage()
            {
                JobId = job.JobId,
                EndTime = DateTimeOffset.Now,
                Success = true
            };

            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<EndJob>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<EndJob>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<EndJob>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<EndJob>>(), Is.False, "message handled without fault");

                await using var actualDbContext = GetDbContext();
                var actual = actualDbContext.Jobs.SingleOrDefault(j => j.JobId == message.JobId);
            
                actual.Should().NotBeNull();
                actual.JobId.Should().Be(message.JobId);
                actual.Status.Should().Be(Status.Completed, "job status is successful");
                actual.EndTime.Should().BeCloseTo(message.EndTime, because: "job start time updated");
            }
            finally
            {
                await Harness.Stop();
            }
        }
    }
}
