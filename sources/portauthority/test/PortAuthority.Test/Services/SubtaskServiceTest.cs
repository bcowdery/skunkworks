using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MassTransit;
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
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Services
{
    public class SubtaskServiceTest
    {
        // class under test
        private SubtaskService _service;

        // mocks
        private readonly MockRepository _mocks = new MockRepository(MockBehavior.Default);
        private Mock<ISendEndpointProvider> _mockSendEndpoint;
        
        [SetUp]
        public void Setup()
        {
            var assembler = new SubtaskModelAssembler();
            var loggerFactory = NullLoggerFactory.Instance;
            var contextFactory = DbContextFactory.Instance;
            
            _mockSendEndpoint = _mocks.Create<ISendEndpointProvider>();
            
            _service = new SubtaskService(
                loggerFactory.CreateLogger<SubtaskService>(),
                contextFactory.CreateDbContext<PortAuthorityDbContext>(),
                _mockSendEndpoint.Object,
                assembler
            );
        }

        [Test]
        public async Task Test_GetTask_Pending_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");
            var task = new SubtaskFaker().SetJob(job).Generate("default,Pending");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);
            
            // act
            var actual = await _service.GetTask(task.TaskId);

            // assert
            actual.Should().NotBeNull();
            actual.IsOk().Should().BeTrue();
            
            var model = actual.Payload;
            model.TaskId.Should().Be(task.TaskId);
            model.Name.Should().Be(task.Name);
            model.Status.Should().Be(Status.Pending);
            model.StartTime.Should().BeNull();
            model.EndTime.Should().BeNull();
        }

        [Test]
        public async Task Test_GetTask_InProgress_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("default,InProgress");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);
            
            // act
            var actual = await _service.GetTask(task.TaskId);

            // assert
            actual.Should().NotBeNull();
            actual.IsOk().Should().BeTrue();
            
            var model = actual.Payload;
            model.TaskId.Should().Be(task.TaskId);
            model.Name.Should().Be(task.Name);
            model.Status.Should().Be(Status.InProgress);
            model.StartTime.Should().BeCloseTo(task.StartTime.Value);
            model.EndTime.Should().BeNull();
        }

        [Test]
        public async Task Test_GetTask_Failed_Should_ReturnModel()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("default,Failed");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);
            
            // act
            var actual = await _service.GetTask(task.TaskId);

            // assert
            actual.Should().NotBeNull();
            actual.IsOk().Should().BeTrue();
            
            var model = actual.Payload;
            model.TaskId.Should().Be(task.TaskId);
            model.Name.Should().Be(task.Name);
            model.Status.Should().Be(Status.Failed);
            model.StartTime.Should().BeCloseTo(task.StartTime.Value);
            model.EndTime.Should().BeCloseTo(task.EndTime.Value);
        }

                
        [Test]
        public async Task Test_GetTask_InvalidId_Should_ReturnNotFoundResult()
        {
            // arrange
            var badId = Guid.NewGuid();
            
            // act
            var result = await _service.GetTask(badId);

            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.Payload.Should().BeNull();
            result.ErrorMessage.Message.Should().StartWith("Subtask not found with ID");
        }

        [Test]
        public async Task Test_GetTask_Empty_Should_ReturnBadRequest()
        {
            // arrange
            var badId = Guid.Empty;
            
            // act
            var result = await _service.GetTask(badId);

            // assert
            result.Should().NotBeNull();
            result.IsBadRequest().Should().BeTrue();
            result.Payload.Should().BeNull();
            result.ErrorMessage.Message.Should().StartWith("Subtask ID cannot be empty");
        }

        [Test]
        public async Task Test_CreateTask_Should_SendMessage_And_ReturnOk()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            
            var form = new CreateSubtaskForm()
            {
                JobId = job.JobId,
                TaskId = NewId.NextGuid(),
                Name = "test-createtask-ok"
            };

            _mockSendEndpoint
                .SetupMessage<CreateSubtask>(new
                {
                    JobId = form.JobId,
                    CorrelationId = form.TaskId,
                    Name = form.Name
                })
                .Verifiable();
                
            // act
            var result = await _service.CreateTask(form);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();
            
            _mockSendEndpoint.Verify();
        }
        
        [Test]
        public async Task Test_CreateTask_JobNotExists_Should_ReturnNotFound()
        {
            // arrange
            var form = new CreateSubtaskForm()
            {
                JobId = Guid.NewGuid(),
                TaskId = NewId.NextGuid(),
                Name = "test-createtask-job-not-exists"
            };

            _mockSendEndpoint
                .SetupMessage<CreateSubtask>(new
                {
                    JobId = form.JobId,
                    CorrelationId = form.TaskId,
                    Name = form.Name
                })
                .Verifiable();
                
            // act
            var result = await _service.CreateTask(form);
            
            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Job does not exist with ID");
        }

        [Test]
        public async Task Test_CreateTask_DuplicateGuid_Should_ReturnConflict()
        {
            // arrange
            var job = new JobFaker().Generate("default,Pending");
            var task = new SubtaskFaker().SetJob(job).Generate("default,Pending");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);
            
            var form = new CreateSubtaskForm()
            {
                JobId = job.JobId,
                TaskId = task.TaskId,
                Name = "test-createtask-task-id-already-exists"
            };

            _mockSendEndpoint
                .SetupMessage<CreateSubtask>(new
                {
                    JobId = form.JobId,
                    CorrelationId = form.TaskId,
                    Name = form.Name
                })
                .Verifiable();
                
            // act
            var result = await _service.CreateTask(form);
            
            // assert
            result.Should().NotBeNull();
            result.IsConflict().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Subtask already exists");
        }

        [Test]
        public async Task Test_StartTask_Should_SendMessage()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("default,Pending");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);            
            
            var startTime = DateTimeOffset.UtcNow;
            
            _mockSendEndpoint
                .SetupMessage<StartSubtask>(new
                {
                    TaskId = task.TaskId,
                    StartTime = startTime
                })
                .Verifiable();
                
            // act
            var result = await _service.StartTask(task.TaskId, startTime);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();
            
            _mockSendEndpoint.Verify();
        }
        
        [Test]
        public async Task Test_StartTask_InvalidId_Should_ReturnNotFound()
        {
            // arrange
            var badId = Guid.NewGuid();
            var startTime = DateTimeOffset.UtcNow;
                
            // act
            var result = await _service.StartTask(badId, startTime);
            
            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Subtask does not exist with ID");
        }
        
        [Test]
        public async Task Test_EndTask_Should_SendMessage()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var task = new SubtaskFaker().SetJob(job).Generate("default,InProgress");
            
            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, task);       

            var endTime = DateTimeOffset.UtcNow;
            var isSuccess = true;

            _mockSendEndpoint
                .SetupMessage<EndSubtask>(new
                {
                    TaskId = task.TaskId,
                    EndTime = endTime,
                    Success = isSuccess
                })
                .Verifiable();
                
            // act
            var result = await _service.EndTask(task.TaskId, endTime, isSuccess);
            
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
            var result = await _service.EndTask(badId, startTime, isSuccess);
            
            // assert
            result.Should().NotBeNull();
            result.IsNotFound().Should().BeTrue();
            result.ErrorMessage.Message.Should().StartWith("Subtask does not exist with ID");
        }
 
        [Test]
        public async Task Test_ListTasks_FindAll_Should_ReturnPagedList()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var tasks = new SubtaskFaker().SetJob(job).Generate(100);

            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, tasks);

            var search = new SubtaskSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };

            // act
            var result = await _service.ListTasks(search, paging);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();

            var payload = result.Payload;
            payload.TotalItems.Should().Be(100);
            payload.TotalPages.Should().Be(4);
            payload.Data.Should().HaveCount(25);
        }
        
        [Test]
        public async Task Test_ListTasks_Find_ByName_Should_ReturnPagedList()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var fooTasks = new SubtaskFaker().SetJob(job).SetName("foo").Generate(10);
            var barTasks = new SubtaskFaker().SetJob(job).SetName("bar").Generate(10);

            await using var context = DbContextFactory.Instance.CreateDbContext<PortAuthorityDbContext>();
            await context.Setup(x => x.Jobs, job);
            await context.Setup(x => x.Tasks, fooTasks);
            await context.Setup(x => x.Tasks, barTasks);

            var search = new SubtaskSearchCriteria() { Name = "bar" };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };

            // act
            var result = await _service.ListTasks(search, paging);
            
            // assert
            result.Should().NotBeNull();
            result.IsOk().Should().BeTrue();

            var payload = result.Payload;
            payload.TotalItems.Should().Be(10);
            payload.TotalPages.Should().Be(1);
            payload.Data.Should().HaveCount(10);
            payload.Data.Should().OnlyContain(x => barTasks.Select(t => t.TaskId).Contains(x.TaskId));
        }
    }
}
