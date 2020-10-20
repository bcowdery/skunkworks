using System;
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
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Consumers
{
    public class CreateSubtaskConsumerTest_Integration
        : ConsumerFixture
    {
        protected override void ConfigureBus(IServiceCollectionBusConfigurator bus)
        {
            bus.AddConsumer<CreateSubtaskConsumer>();
        }

        [Test]
        public async Task Test_CreateSubtaskConsumer_Should_ConsumeMessage_And_CreateJob()
        {
            // arrange
            var consumerHarness = Consumer<CreateSubtaskConsumer>();

            var job = new JobFaker().Generate("default,InProgress");
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            
            var message = new TestCreateSubtaskMessage()
            {
                JobId = job.JobId,
                TaskId = NewId.NextGuid(),
                Name = "test-create-subtask"
            };
            
            await Harness.Start();
            
            try
            {
                // act
                await Harness.InputQueueSendEndpoint.Send<CreateSubtask>(message);

                // assert
                Assert.That(await Harness.Consumed.Any<CreateSubtask>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<CreateSubtask>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<CreateSubtask>>(), Is.False, "message handled without fault");

                var actual = GetDbContext().Tasks.SingleOrDefault(t => t.TaskId == message.TaskId);
                
                actual.Should().NotBeNull();
                actual.TaskId.Should().Be(message.TaskId);
                actual.Name.Should().Be(message.Name);
            }
            finally
            {
                await Harness.Stop();
            }
        }    
    }
}
