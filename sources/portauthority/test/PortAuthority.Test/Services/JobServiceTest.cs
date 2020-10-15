using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PortAuthority.Assemblers;
using PortAuthority.Data;
using PortAuthority.Extensions;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Mocks;

namespace PortAuthority.Test.Services
{
    public class JobServiceTest
    {
        // class under test
        private JobService _service;

        // mocks
        private readonly MockRepository _mocks = new MockRepository(MockBehavior.Default);
        private Mock<ISendEndpoint> _mockSendEndpoint;
        
        [SetUp]
        public void Setup()
        {
            var assembler = new JobModelAssembler();
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _mockSendEndpoint = _mocks.Create<ISendEndpoint>();
            
            _service = new JobService(
                loggerFactory.CreateLogger<JobService>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>(),
                _mockSendEndpoint.Object,
                assembler
            );
        }

        [Test]
        public async Task Test_GetJob_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate();

            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, new []{ job });
            
            // act
            var actual = await _service.GetJob(job.JobId);

            // assert
            actual.Should().NotBeNull();
            actual.IsOk().Should().BeTrue();
            
            var model = actual.Payload;
            model.JobId.Should().Be(job.JobId);
            model.CorrelationId.Should().Be(job.CorrelationId);
            model.Type.Should().Be(job.Type);
            model.Namespace.Should().Be(job.Namespace);
            model.StartTime.Should().Be(job.StartTime);
            model.EndTime.Should().Be(job.EndTime);
        }

        [Test]
        public async Task Test_GetJob_InvalidId_Should_ReturnNotFoundResult()
        {
            // arrange
            var badId = Guid.NewGuid();
            
            // act
            var actual = await _service.GetJob(badId);

            // assert
            actual.Should().NotBeNull();
            actual.IsNotFound().Should().BeTrue();
            actual.Payload.Should().BeNull();
        }

        [Test]
        public async Task Test_GetJob_Empty_Should_ReturnBadRequest()
        {
            // arrange
            var badId = Guid.Empty;
            
            // act
            var actual = await _service.GetJob(badId);

            // assert
            actual.Should().NotBeNull();
            actual.IsBadRequest().Should().BeTrue();
            actual.Payload.Should().BeNull();
        }
    }
}
