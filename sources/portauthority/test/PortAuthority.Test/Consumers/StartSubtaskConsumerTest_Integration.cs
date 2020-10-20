using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using NUnit.Framework;
using PortAuthority.Consumers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class StartSubtaskConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<StartSubtaskConsumer>();
        }

        [Test]
        public async Task Test_StartSubtaskConsumer_Should_Consume_With_Status_InProgress()
        {
            // arrange
            var consumerHarness = Consumer<StartSubtaskConsumer>();
            
            var job = new JobFaker().Generate("default,Pending");
            var task = new SubtaskFaker().SetJob(job).Generate("default,Pending");

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestStartSubtaskMessage()
            {
                TaskId = task.TaskId,
                StartTime = DateTimeOffset.Now
            };
            
            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<StartSubtask>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<StartSubtask>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<StartSubtask>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<StartSubtask>>(), Is.False, "message handled without fault");

                await using var actualDbContext = GetDbContext();
                var actual = actualDbContext.Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
                
                actual.Should().NotBeNull();
                actual.TaskId.Should().Be(task.TaskId);
                actual.Status.Should().Be(Status.InProgress, "job status updated");
                actual.StartTime.Should().BeCloseTo(message.StartTime, because: "job start time updated");
            }
            finally
            {
                await Harness.Stop();
            }
        }    
    }
}
