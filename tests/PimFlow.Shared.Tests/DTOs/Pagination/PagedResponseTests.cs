using FluentAssertions;
using PimFlow.Shared.DTOs.Pagination;
using Xunit;

namespace PimFlow.Shared.Tests.DTOs.Pagination;

public class PagedResponseTests
{
    [Fact]
    public void Constructor_WithoutParameters_ShouldSetDefaultValues()
    {
        // Act
        var response = new PagedResponse<string>();

        // Assert
        response.Items.Should().NotBeNull().And.BeEmpty();
        response.PageNumber.Should().Be(0);
        response.PageSize.Should().Be(0);
        response.TotalCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithParameters_ShouldSetValues()
    {
        // Arrange
        var items = new[] { "item1", "item2", "item3" };
        var pageNumber = 2;
        var pageSize = 10;
        var totalCount = 25;

        // Act
        var response = new PagedResponse<string>(items, pageNumber, pageSize, totalCount);

        // Assert
        response.Items.Should().BeEquivalentTo(items);
        response.PageNumber.Should().Be(pageNumber);
        response.PageSize.Should().Be(pageSize);
        response.TotalCount.Should().Be(totalCount);
    }

    [Theory]
    [InlineData(25, 10, 3)]
    [InlineData(30, 10, 3)]
    [InlineData(31, 10, 4)]
    [InlineData(100, 25, 4)]
    [InlineData(101, 25, 5)]
    [InlineData(0, 10, 0)]
    [InlineData(5, 10, 1)]
    public void TotalPages_ShouldCalculateCorrectly(int totalCount, int pageSize, int expectedTotalPages)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            TotalCount = totalCount,
            PageSize = pageSize
        };

        // Act & Assert
        response.TotalPages.Should().Be(expectedTotalPages);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(10, true)]
    public void HasPreviousPage_ShouldReturnCorrectValue(int pageNumber, bool expectedResult)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber
        };

        // Act & Assert
        response.HasPreviousPage.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, 3, true)]
    [InlineData(2, 3, true)]
    [InlineData(3, 3, false)]
    [InlineData(4, 3, false)]
    public void HasNextPage_ShouldReturnCorrectValue(int pageNumber, int totalPages, bool expectedResult)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber,
            TotalCount = totalPages * 10,
            PageSize = 10
        };

        // Act & Assert
        response.HasNextPage.Should().Be(expectedResult);
    }

    [Fact]
    public void FirstPage_ShouldAlwaysReturnOne()
    {
        // Arrange
        var response = new PagedResponse<string>();

        // Act & Assert
        response.FirstPage.Should().Be(1);
    }

    [Theory]
    [InlineData(25, 10, 3)]
    [InlineData(30, 10, 3)]
    [InlineData(31, 10, 4)]
    [InlineData(0, 10, 0)]
    public void LastPage_ShouldReturnTotalPages(int totalCount, int pageSize, int expectedLastPage)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            TotalCount = totalCount,
            PageSize = pageSize
        };

        // Act & Assert
        response.LastPage.Should().Be(expectedLastPage);
    }

    [Theory]
    [InlineData(1, null)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(10, 9)]
    public void PreviousPage_ShouldReturnCorrectValue(int pageNumber, int? expectedPreviousPage)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber
        };

        // Act & Assert
        response.PreviousPage.Should().Be(expectedPreviousPage);
    }

    [Theory]
    [InlineData(1, 3, 2)]
    [InlineData(2, 3, 3)]
    [InlineData(3, 3, null)]
    [InlineData(4, 3, null)]
    public void NextPage_ShouldReturnCorrectValue(int pageNumber, int totalPages, int? expectedNextPage)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber,
            TotalCount = totalPages * 10,
            PageSize = 10
        };

        // Act & Assert
        response.NextPage.Should().Be(expectedNextPage);
    }

    [Theory]
    [InlineData(1, 10, 25, 1)]
    [InlineData(2, 10, 25, 11)]
    [InlineData(3, 10, 25, 21)]
    [InlineData(1, 5, 0, 0)]
    public void FirstItemIndex_ShouldCalculateCorrectly(int pageNumber, int pageSize, int totalCount, int expectedFirstIndex)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        // Act & Assert
        response.FirstItemIndex.Should().Be(expectedFirstIndex);
    }

    [Theory]
    [InlineData(1, 10, 25, 10)]
    [InlineData(2, 10, 25, 20)]
    [InlineData(3, 10, 25, 25)]
    [InlineData(1, 10, 5, 5)]
    [InlineData(1, 10, 0, 0)]
    public void LastItemIndex_ShouldCalculateCorrectly(int pageNumber, int pageSize, int totalCount, int expectedLastIndex)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        // Act & Assert
        response.LastItemIndex.Should().Be(expectedLastIndex);
    }

    [Theory]
    [InlineData(0, "No hay elementos")]
    [InlineData(25, "Mostrando 1-10 de 25 elementos")]
    [InlineData(5, "Mostrando 1-5 de 5 elementos")]
    public void PaginationInfo_ShouldReturnCorrectText(int totalCount, string expectedInfo)
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = totalCount
        };

        // Act & Assert
        response.PaginationInfo.Should().Be(expectedInfo);
    }

    [Fact]
    public void Empty_ShouldCreateEmptyResponse()
    {
        // Act
        var response = PagedResponse<string>.Empty();

        // Assert
        response.Items.Should().BeEmpty();
        response.PageNumber.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalCount.Should().Be(0);
        response.TotalPages.Should().Be(0);
    }

    [Fact]
    public void Empty_WithParameters_ShouldCreateEmptyResponseWithSpecifiedValues()
    {
        // Act
        var response = PagedResponse<string>.Empty(3, 25);

        // Assert
        response.Items.Should().BeEmpty();
        response.PageNumber.Should().Be(3);
        response.PageSize.Should().Be(25);
        response.TotalCount.Should().Be(0);
        response.TotalPages.Should().Be(0);
    }

    [Fact]
    public void Create_ShouldCreatePagedResponseFromCollection()
    {
        // Arrange
        var allItems = Enumerable.Range(1, 25).Select(i => $"Item {i}").ToList();
        var pageNumber = 2;
        var pageSize = 10;

        // Act
        var response = PagedResponse<string>.Create(allItems, pageNumber, pageSize);

        // Assert
        response.Items.Should().HaveCount(10);
        response.Items.First().Should().Be("Item 11");
        response.Items.Last().Should().Be("Item 20");
        response.PageNumber.Should().Be(pageNumber);
        response.PageSize.Should().Be(pageSize);
        response.TotalCount.Should().Be(25);
        response.TotalPages.Should().Be(3);
    }

    [Fact]
    public void Create_WithLastPartialPage_ShouldReturnCorrectItems()
    {
        // Arrange
        var allItems = Enumerable.Range(1, 23).Select(i => $"Item {i}").ToList();
        var pageNumber = 3;
        var pageSize = 10;

        // Act
        var response = PagedResponse<string>.Create(allItems, pageNumber, pageSize);

        // Assert
        response.Items.Should().HaveCount(3);
        response.Items.First().Should().Be("Item 21");
        response.Items.Last().Should().Be("Item 23");
        response.TotalCount.Should().Be(23);
        response.TotalPages.Should().Be(3);
    }
}
