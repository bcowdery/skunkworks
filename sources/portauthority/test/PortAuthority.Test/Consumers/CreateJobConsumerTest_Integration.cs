using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using NUnit.Framework;
using PortAuthority.Contracts.Commands;
using PortAuthority.Worker.Consumer;

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
            var consumerHarness = Consumer<CreateJobConsumer>();
            
            await Harness.Start();
            try
            {
                await Harness.InputQueueSendEndpoint.Send<CreateJob>(new 
                {
                    JobId = NewId.NextGuid(),
                    Type = "foo-bar",
                    Namespace = "com.portauthority"
                });

                Assert.That(await Harness.Consumed.Any<CreateJob>(), "endpoint consumed message");
                Assert.That(await consumerHarness.Consumed.Any<CreateJob>(), "actual consumer consumed the message");
                Assert.That(await Harness.Published.Any<Fault<CreateJob>>(), Is.False, "message handled without fault");
            }
            finally
            {
                await Harness.Stop();
            }
        }    
    }
}
