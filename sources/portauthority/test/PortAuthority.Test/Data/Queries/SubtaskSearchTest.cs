using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PortAuthority.Data.Queries;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Utils;

namespace PortAuthority.Test.Data.Queries
{
    public class SubtaskSearchQueryTest 
        : DatabaseFixture
    {
        /* delete data between each test */
        
        [SetUp]
        public async Task SetupDatabase()
        {
            await EnsureCreated();
        }
        
        [TearDown]
        public async Task ClearDatabase()
        {
            await EnsureDeleted(); 
        }
        
        [Test]
        public async Task Test_SubtaskSearchQuery_NoData_Should_ReturnEmptyResult()
        {
            // arrange
            var search = new SubtaskSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var dbContext = GetDbContext();
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(0);
            results.TotalPages.Should().Be(0);            
            results.Data.Should().BeNullOrEmpty();
        }
        
        [Test]
        public async Task Test_SubtaskSearchQuery_DefaultCriteria_Should_ReturnAll()
        {
            // arrange
            var job = new JobFaker().Generate();
            var tasks = new SubtaskFaker().SetJob(job).Generate(100);

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, tasks);
            
            var search = new SubtaskSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(100);
            results.TotalPages.Should().Be(4);
            results.Data.Should().HaveCount(25);
            results.Data.Should().OnlyHaveUniqueItems(x => x.TaskId);
        }
        
        [Test]
        public async Task Test_SubtaskSearchQuery_FindByName_Should_ReturnMatching()
        {
            // arrange
            var expectedName = "find-by-name";
            
            var job = new JobFaker().Generate("default,InProgress");
            var tasks = new SubtaskFaker().SetJob(job).Generate(100);
            var tasksToFind = new SubtaskFaker().SetJob(job).SetName(expectedName).Generate(4); 
                
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, tasks);
            await dbContext.Setup(x => x.Tasks, tasksToFind);
            
            var search = new SubtaskSearchCriteria() { Name = expectedName };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(4);
            results.TotalPages.Should().Be(1);
            results.Data.Should().HaveCount(4);
            results.Data.Should().OnlyContain(x => x.Name == expectedName);
        }        

        [Test]
        public async Task Test_SubtaskSearchQuery_FindByJobId_Should_ReturnMatching()
        {
            // arrange
            var jobA = new JobFaker().Generate("default,InProgress");
            var jobB = new JobFaker().Generate("default,InProgress");
            
            var aTasks = new SubtaskFaker().SetJob(jobA).Generate(100);
            var bTasks  = new SubtaskFaker().SetJob(jobB).Generate(23);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, new [] { jobA, jobB });
            await dbContext.Setup(x => x.Tasks, aTasks);
            await dbContext.Setup(x => x.Tasks, bTasks);
            
            var search = new SubtaskSearchCriteria() { JobId = jobB.JobId };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(23);
            results.TotalPages.Should().Be(1);
            results.Data.Should().HaveCount(23);
            results.Data.Should().OnlyContain(x => bTasks.Select(t => t.TaskId).Contains(x.TaskId));
        }            
        
        [Test]
        public async Task Test_SubtaskSearchQuery_FindByJobId_None_Should_ReturnEmpty()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var tasks = new SubtaskFaker().SetJob(job).Generate(17);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, tasks);
            
            var search = new SubtaskSearchCriteria() { JobId = Guid.Empty };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(0);
            results.TotalPages.Should().Be(0);
            results.Data.Should().BeNullOrEmpty();
        }  
        

        [Test]
        public async Task Test_SubtaskSearchQuery_TinyPageSize_Should_ReturnPagedResult()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var tasks = new SubtaskFaker().SetJob(job).Generate(100);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, tasks);

            var search = new SubtaskSearchCriteria();
            var paging = new PagingCriteria() { Page = 1, Size = 5 };
            
            // act
            var results = await new SubtaskSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(5);
            results.TotalItems.Should().Be(100);
            results.TotalPages.Should().Be(20);
            results.Data.Should().HaveCount(5);
        }         
        
        [Test]
        public async Task Test_SubtaskSearchQuery_PageForward_Should_ReturnNewPage()
        {
            // arrange
            var job = new JobFaker().Generate("default,InProgress");
            var tasks = new SubtaskFaker().SetJob(job).Generate(100);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, job);
            await dbContext.Setup(x => x.Tasks, tasks);

            var search = new SubtaskSearchCriteria();
            
            // act
            var query = new SubtaskSearchQuery(dbContext);
            var page1 = await query.Find(search, new PagingCriteria() { Page = 1, Size = 5 });
            var page2 = await query.Find(search, new PagingCriteria() { Page = 2, Size = 5 });
            var page3 = await query.Find(search, new PagingCriteria() { Page = 3, Size = 5 });
            var page10 = await query.Find(search, new PagingCriteria() { Page = 10, Size = 5 });
            
            // assert
            page1.Should().NotBeNull();
            page1.Page.Should().Be(1);
            page1.TotalItems.Should().Be(100);
            page1.TotalPages.Should().Be(20);
            page1.Data.Should()
                .NotContain(page2.Data)
                .And.NotContain(page3.Data)
                .And.NotContain(page10.Data);
            
            page2.Should().NotBeNull();
            page2.Page.Should().Be(2);
            page2.TotalItems.Should().Be(100);
            page2.TotalPages.Should().Be(20);
            page2.Data.Should()
                .NotContain(page1.Data)
                .And.NotContain(page3.Data)
                .And.NotContain(page10.Data);            
            
            page3.Should().NotBeNull();
            page3.Page.Should().Be(3);
            page3.TotalItems.Should().Be(100);
            page3.TotalPages.Should().Be(20);
            page3.Data.Should()
                .NotContain(page1.Data)
                .And.NotContain(page2.Data)
                .And.NotContain(page10.Data);
            
            page10.Should().NotBeNull();
            page10.Page.Should().Be(10);
            page10.TotalItems.Should().Be(100);
            page10.TotalPages.Should().Be(20);
            page10.Data.Should()
                .NotContain(page1.Data)
                .And.NotContain(page2.Data)
                .And.NotContain(page3.Data);
        }                           
    }
}
