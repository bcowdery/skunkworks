using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Testing;
using NUnit.Framework;
using PortAuthority.Consumers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data.Entities;
using PortAuthority.Test.Consumers.TestMessages;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class EndSubtaskConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<EndSubtaskConsumer>();
        }
        
        [Test]
        public async Task Test_EndSubtaskConsumer_Success_True_Should_Consume_With_Status_Completed()
        {
            // arrange
            var consumerHarness = Consumer<EndSubtaskConsumer>();

            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("default,InProgress");

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, task);
            
            var message = new TestEndSubtaskMessage()
            {
                TaskId = task.TaskId,
                EndTime = DateTimeOffset.Now,
                Success = true
            };

            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<EndSubtask>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<EndSubtask>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<EndSubtask>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<EndSubtask>>(), Is.False, "message handled without fault");

                await using var actualDbContext = GetDbContext();
                var actual = actualDbContext.Tasks.SingleOrDefault(t => t.TaskId == task.TaskId);
            
                actual.Should().NotBeNull();
                actual.TaskId.Should().Be(task.TaskId);
                actual.Status.Should().Be(Status.Completed, "task status is successful");
                actual.EndTime.Should().BeCloseTo(message.EndTime, because: "task end time updated");
            }
            finally
            {
                await Harness.Stop();
            }
        }
    }
}
