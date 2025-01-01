using Chatty.Core.Application.Common.Pagination;
using FluentAssertions;
using Xunit;

namespace Chatty.Core.Application.Tests.Unit;

public class PaginationResultTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var data = new List<string> { "Item1", "Item2", "Item3" };
        var totalRecords = 10;
        var page = 2;
        var pageSize = 3;

        // Act
        var result = new PaginationResult<string>(data, totalRecords, page, pageSize);

        // Assert
        result.Data.Should().BeEquivalentTo(data);
        result.TotalRecords.Should().Be(totalRecords);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.TotalPages.Should().Be(4); // Ceiling of 10 / 3
    }

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        // Arrange
        var data = new List<string>();
        var totalRecords = 20;
        var page = 1;
        var pageSize = 6;

        // Act
        var result = new PaginationResult<string>(data, totalRecords, page, pageSize);

        // Assert
        result.TotalPages.Should().Be(4); // Ceiling of 20 / 6
    }
}