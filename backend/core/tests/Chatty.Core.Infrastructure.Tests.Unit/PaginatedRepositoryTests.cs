using Chatty.Core.Application.Common.Pagination;
using Chatty.Core.Domain.Models;
using Chatty.Core.Infrastructure.Persistance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Sensei.Core.Infrastructure.Tests.Unit;

public class PaginatedRepositoryTests
{
    private readonly PaginatedRepository<TestEntity> _paginatedRepository;
    private readonly DbContextOptions<TestDbContext> _dbContextOptions;

    public PaginatedRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _paginatedRepository = new PaginatedRepository<TestEntity>();
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnCorrectPaginationResult()
    {
        // Arrange
        using var context = new TestDbContext(_dbContextOptions);
        await SeedTestData(context);

        var query = context.TestEntities.AsQueryable();
        var paginationQuery = new PaginationQuery { Page = 2, PageSize = 3 };

        // Act
        var result = await _paginatedRepository.GetPaginatedAsync(paginationQuery, query, CancellationToken.None);

        // Assert
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(3);
        result.TotalRecords.Should().Be(10);
        result.TotalPages.Should().Be(4);
        result.Data.Should().HaveCount(3);
    }

    private async Task SeedTestData(TestDbContext context)
    {
        for (int i = 1; i <= 10; i++)
        {
            context.TestEntities.Add(new TestEntity { Name = $"Entity {i}" });
        }
        await context.SaveChangesAsync();
    }

    private class TestEntity : EntityBase
    {
        public TestEntity(): base(Guid.NewGuid()) { }
        public string Name { get; set; } = string.Empty;
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }
}