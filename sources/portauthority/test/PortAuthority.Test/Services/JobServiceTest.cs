﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PortAuthority.Assemblers;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Data.Entities;
using PortAuthority.Data.Queries;
using PortAuthority.Extensions;
using PortAuthority.Forms;
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
        public async Task Test_GetJob_Pending_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");

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
            model.Status.Should().Be(Status.Pending);
            model.StartTime.Should().BeNull();
            model.EndTime.Should().BeNull();
        }

        [Test]
        public async Task Test_GetJob_InProgress_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");

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
            model.Status.Should().Be(Status.InProgress);
            model.StartTime.Should().Be(job.StartTime);
            model.EndTime.Should().BeNull();
        }
        
        [Test]
        public async Task Test_GetJob_Failed_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,Failed");

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
            model.Status.Should().Be(Status.Failed);
            model.StartTime.Should().Be(job.StartTime);
            model.EndTime.Should().Be(job.EndTime);
        }

        [Test]
        public async Task Test_GetJob_InvalidId_Should_ReturnNotFoundResult()
        {
            // arrange
            var badId = Guid.NewGuid();
            
            // act
            var result = await _service.GetJob(badId);

            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.Payload.Should().BeNull();
            result.ErrorMessage.Message.Should().StartWith("Job not found with ID");
        }

        [Test]
        public async Task Test_GetJob_Empty_Should_ReturnBadRequest()
        {
            // arrange
            var badId = Guid.Empty;
            
            // act
            var result = await _service.GetJob(badId);

            // assert
            result.Should().NotBeNull();
            result.IsBadRequest().Should().BeTrue();
            result.Payload.Should().BeNull();
            result.ErrorMessage.Message.Should().StartWith("Job ID cannot be empty");
        }

        [Test]
        public async Task Test_CreateJob_Should_SendMessage_And_ReturnOk()
        {
            // arrange
            var faker = new Faker();
            var form = new CreateJobForm()
            {
                JobId = NewId.NextGuid(),
                Type = faker.Lorem.Slug(),
                Namespace = faker.Internet.DomainName()
            };

            _mockSendEndpoint
                .SetupMessage<CreateJob>(new
                {
                    JobId = form.JobId,
                    CorrelationId = form.CorrelationId,
                    Type = form.Type,
                    Namespace = form.Namespace
                })
                .Verifiable();
                
            // act
            var result = await _service.CreateJob(form);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();
            
            _mockSendEndpoint.Verify();
        }

        [Test]
        public async Task Test_CreateJob_DuplicateGuid_Should_ReturnConflict()
        {
            // arrange
            var job = new JobFaker().Generate();

            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, new []{ job });
            
            var form = new CreateJobForm()
            {
                JobId = job.JobId,
                Type = "foo-bar-baz",
                Namespace = "com.portauthority"
            };
            
            // act
            var result = await _service.CreateJob(form);

            // assert
            result.Should().NotBeNull();
            result.IsConflict().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Job already exists");
        }

        [Test]
        public async Task Test_StartJob_Should_SendMessage()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");
            var startTime = DateTimeOffset.UtcNow;
            
            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, new []{ job });
            
            _mockSendEndpoint
                .SetupMessage<StartJob>(new
                {
                    JobId = job.JobId,
                    StartTime = startTime
                })
                .Verifiable();
                
            // act
            var result = await _service.StartJob(job.JobId, startTime);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();
            
            _mockSendEndpoint.Verify();
        }
        
        [Test]
        public async Task Test_StartJob_InvalidId_Should_ReturnNotFound()
        {
            // arrange
            var badId = Guid.NewGuid();
            var startTime = DateTimeOffset.UtcNow;
                
            // act
            var result = await _service.StartJob(badId, startTime);
            
            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Job does not exist with ID");
        }
        
        [Test]
        public async Task Test_EndJob_Should_SendMessage()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var endTime = DateTimeOffset.UtcNow;
            var isSuccess = true;
            
            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, new []{ job });
            
            _mockSendEndpoint
                .SetupMessage<EndJob>(new
                {
                    JobId = job.JobId,
                    EndTime = endTime,
                    Success = isSuccess
                })
                .Verifiable();
                
            // act
            var result = await _service.EndJob(job.JobId, endTime, isSuccess);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();
            
            _mockSendEndpoint.Verify();
        }
        
        [Test]
        public async Task Test_EndJob_InvalidId_Should_ReturnNotFound()
        {
            // arrange
            var badId = Guid.NewGuid();
            var startTime = DateTimeOffset.UtcNow;
            var isSuccess = false;
                
            // act
            var result = await _service.EndJob(badId, startTime, isSuccess);
            
            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Job does not exist with ID");
        }

        [Test]
        public async Task Test_ListJobs_FindAll_Should_ReturnPagedList()
        {
            // arrange
            var jobs = new JobFaker()
                .Generate(100);

            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, jobs);

            var search = new JobSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };

            // act
            var result = await _service.ListJobs(search, paging);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();

            var payload = result.Payload;
            payload.TotalItems.Should().Be(100);
            payload.TotalPages.Should().Be(4);
            payload.Data.Should().HaveCount(25);
        }
        
        [Test]
        public async Task Test_ListJobs_Find_ByCorrelationId_Should_ReturnPagedList()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var bulkJobs = new JobFaker().Generate(100);
            var relatedJobs = new JobFaker().SetCorrelationId(correlationId).Generate(10);

            using var context = DbContextFactory.Instance.CreateTestable<PortAuthorityDbContext>();
            context.Setup(x => x.Jobs, bulkJobs);
            context.Setup(x => x.Jobs, relatedJobs);

            var search = new JobSearchCriteria() { CorrelationId = correlationId};
            var paging = new PagingCriteria() { Page = 1, Size = 25 };

            // act
            var result = await _service.ListJobs(search, paging);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();

            var payload = result.Payload;
            payload.TotalItems.Should().Be(10);
            payload.TotalPages.Should().Be(1);
            payload.Data.Should().HaveCount(10);
            payload.Data.Should().OnlyContain(x => relatedJobs.Select(j => j.JobId).Contains(x.JobId));
        }
    }
}
