using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PortAuthority.Data.Queries;
using PortAuthority.Test.Fakes;
using PortAuthority.Test.Mocks;

namespace PortAuthority.Test.Data.Queries
{
    public class JobSearchQueryTest : DatabaseFixture
    {
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
        public async Task Test_JobSearchQuery_NoData_Should_ReturnEmptyResult()
        {
            // arrange
            var search = new JobSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var dbContext = GetDbContext();
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(0);
            results.TotalPages.Should().Be(0);            
            results.Data.Should().BeNullOrEmpty();
        }
        
        [Test]
        public async Task Test_JobSearchQuery_DefaultCriteria_Should_ReturnAll()
        {
            // arrange
            var jobs = new JobFaker().Generate(100);

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, jobs);
            
            var search = new JobSearchCriteria() { };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(100);
            results.TotalPages.Should().Be(4);
            results.Data.Should().HaveCount(25);
            results.Data.Should().OnlyHaveUniqueItems(x => x.JobId);
        }
        
        [Test]
        public async Task Test_JobSearchQuery_FindByTypeAndNamespace_Should_ReturnMatching()
        {
            // arrange
            var jobs = new JobFaker().Generate(100);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, jobs);
            
            var expected = jobs[39];
            var search = new JobSearchCriteria() { Type = expected.Type, Namespace = expected.Namespace };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(1);
            results.TotalPages.Should().Be(1);
            results.Data.Should().HaveCount(1);
            results.Data.Should().OnlyContain(x => x.Type == expected.Type && x.Namespace == expected.Namespace);
        }        

        [Test]
        public async Task Test_JobSearchQuery_FindByCorrelationId_Should_ReturnMatching()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var bulkJobs = new JobFaker().Generate(100);
            var relatedJobs  = new JobFaker().SetCorrelationId(correlationId).Generate(13);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, bulkJobs);
            await dbContext.Setup(x => x.Jobs, relatedJobs);
            
            var search = new JobSearchCriteria() { CorrelationId = correlationId };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(13);
            results.TotalPages.Should().Be(1);
            results.Data.Should().HaveCount(13);
            results.Data.Should().OnlyContain(x => relatedJobs.Select(j => j.JobId).Contains(x.JobId));
        }            
        
        [Test]
        public async Task Test_JobSearchQuery_FindByCorrelationId_None_Should_ReturnEmpty()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var bulkJobs = new JobFaker().Generate(100);

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, bulkJobs);
            
            var search = new JobSearchCriteria() { CorrelationId = correlationId };
            var paging = new PagingCriteria() { Page = 1, Size = 25 };
            
            // act
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(25);
            results.TotalItems.Should().Be(0);
            results.TotalPages.Should().Be(0);
            results.Data.Should().BeNullOrEmpty();
        }  
        

        [Test]
        public async Task Test_JobSearchQuery_TinyPageSize_Should_ReturnPagedResult()
        {
            // arrange
            var jobs = new JobFaker().Generate(100);

            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, jobs);

            var search = new JobSearchCriteria();
            var paging = new PagingCriteria() { Page = 1, Size = 5 };
            
            // act
            var results = await new JobSearchQuery(dbContext).Find(search, paging);
            
            // assert
            results.Should().NotBeNull();
            results.Page.Should().Be(1);
            results.Size.Should().Be(5);
            results.TotalItems.Should().Be(100);
            results.TotalPages.Should().Be(20);
            results.Data.Should().HaveCount(5);
        }         
        
        [Test]
        public async Task Test_JobSearchQuery_PageForward_Should_ReturnNewPage()
        {
            // arrange
            var jobs = new JobFaker().Generate(100);
            
            await using var dbContext = GetDbContext();
            await dbContext.Setup(x => x.Jobs, jobs);

            var search = new JobSearchCriteria();
            
            // act
            var query = new JobSearchQuery(dbContext);
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
