using FluentAssertions;
using PimFlow.Shared.DTOs.Pagination;
using Xunit;

namespace PimFlow.Shared.Tests.DTOs.Pagination;

public class PagedRequestTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var request = new PagedRequest();

        // Assert
        request.PageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
        request.SearchTerm.Should().BeNull();
        request.SortBy.Should().BeNull();
        request.SortDirection.Should().Be("asc");
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(-10, 1)]
    public void PageNumber_WhenSetToInvalidValue_ShouldDefaultToOne(int invalidValue, int expectedValue)
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        request.PageNumber = invalidValue;

        // Assert
        request.PageNumber.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(100, 100)]
    public void PageNumber_WhenSetToValidValue_ShouldRetainValue(int validValue, int expectedValue)
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        request.PageNumber = validValue;

        // Assert
        request.PageNumber.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(-10, 10)]
    public void PageSize_WhenSetToInvalidValue_ShouldDefaultToTen(int invalidValue, int expectedValue)
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        request.PageSize = invalidValue;

        // Assert
        request.PageSize.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(101, 100)]
    [InlineData(200, 100)]
    [InlineData(1000, 100)]
    public void PageSize_WhenSetToValueAboveLimit_ShouldCapAtOneHundred(int invalidValue, int expectedValue)
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        request.PageSize = invalidValue;

        // Assert
        request.PageSize.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    public void PageSize_WhenSetToValidValue_ShouldRetainValue(int validValue, int expectedValue)
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        request.PageSize = validValue;

        // Assert
        request.PageSize.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 10, 20)]
    [InlineData(1, 25, 0)]
    [InlineData(2, 25, 25)]
    [InlineData(5, 20, 80)]
    public void Skip_ShouldCalculateCorrectly(int pageNumber, int pageSize, int expectedSkip)
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act & Assert
        request.Skip.Should().Be(expectedSkip);
    }

    [Theory]
    [InlineData("asc", false)]
    [InlineData("ASC", false)]
    [InlineData("desc", true)]
    [InlineData("DESC", true)]
    [InlineData("Desc", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsDescending_ShouldReturnCorrectValue(string? sortDirection, bool expectedResult)
    {
        // Arrange
        var request = new PagedRequest
        {
            SortDirection = sortDirection
        };

        // Act & Assert
        request.IsDescending.Should().Be(expectedResult);
    }

    [Fact]
    public void SearchTerm_ShouldAcceptNullAndStringValues()
    {
        // Arrange
        var request = new PagedRequest();

        // Act & Assert - Null
        request.SearchTerm = null;
        request.SearchTerm.Should().BeNull();

        // Act & Assert - String
        request.SearchTerm = "test search";
        request.SearchTerm.Should().Be("test search");

        // Act & Assert - Empty string
        request.SearchTerm = "";
        request.SearchTerm.Should().Be("");
    }

    [Fact]
    public void SortBy_ShouldAcceptNullAndStringValues()
    {
        // Arrange
        var request = new PagedRequest();

        // Act & Assert - Null
        request.SortBy = null;
        request.SortBy.Should().BeNull();

        // Act & Assert - String
        request.SortBy = "name";
        request.SortBy.Should().Be("name");

        // Act & Assert - Empty string
        request.SortBy = "";
        request.SortBy.Should().Be("");
    }
}
