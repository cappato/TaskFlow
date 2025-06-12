using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Server.Controllers;
using PimFlow.Server.Services;
using PimFlow.Shared.Common;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;
using Xunit;

namespace PimFlow.Server.Tests.Controllers;

public class ArticlesPaginationControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly Mock<ILogger<ArticlesController>> _mockLogger;
    private readonly ArticlesController _controller;

    public ArticlesPaginationControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _mockLogger = new Mock<ILogger<ArticlesController>>();
        _controller = new ArticlesController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetArticlesPaged_WithDefaultParameters_ShouldReturnSuccessResponse()
    {
        // Arrange
        var expectedResponse = CreateTestPagedResponse(10, 1, 10, 25);
        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetArticlesPaged();

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = actionResult.Value.Should().BeOfType<ApiResponse<PagedResponse<ArticleDto>>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.PageNumber.Should().Be(1);
        apiResponse.Data.PageSize.Should().Be(10);
    }

    [Theory]
    [InlineData(2, 25)]
    [InlineData(1, 5)]
    [InlineData(3, 50)]
    public async Task GetArticlesPaged_WithCustomPageParameters_ShouldPassCorrectRequest(int pageNumber, int pageSize)
    {
        // Arrange
        var expectedResponse = CreateTestPagedResponse(pageSize, pageNumber, pageSize, 100);
        PagedRequest? capturedRequest = null;

        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .Callback<PagedRequest>(req => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(pageNumber, pageSize);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.PageNumber.Should().Be(pageNumber);
        capturedRequest.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public async Task GetArticlesPaged_WithSearchTerm_ShouldPassSearchTermToService()
    {
        // Arrange
        var searchTerm = "Nike";
        var expectedResponse = CreateTestPagedResponse(5, 1, 10, 5);
        PagedRequest? capturedRequest = null;

        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .Callback<PagedRequest>(req => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(searchTerm: searchTerm);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.SearchTerm.Should().Be(searchTerm);
    }

    [Theory]
    [InlineData("name", "asc")]
    [InlineData("sku", "desc")]
    [InlineData("brand", "asc")]
    [InlineData("createdat", "desc")]
    public async Task GetArticlesPaged_WithSortParameters_ShouldPassSortingToService(string sortBy, string sortDirection)
    {
        // Arrange
        var expectedResponse = CreateTestPagedResponse(10, 1, 10, 25);
        PagedRequest? capturedRequest = null;

        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .Callback<PagedRequest>(req => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(sortBy: sortBy, sortDirection: sortDirection);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.SortBy.Should().Be(sortBy);
        capturedRequest.SortDirection.Should().Be(sortDirection);
    }

    [Fact]
    public async Task GetArticlesPaged_WithAllParameters_ShouldPassAllParametersToService()
    {
        // Arrange
        var pageNumber = 3;
        var pageSize = 25;
        var searchTerm = "running shoes";
        var sortBy = "name";
        var sortDirection = "desc";
        var expectedResponse = CreateTestPagedResponse(25, 3, 25, 100);
        PagedRequest? capturedRequest = null;

        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .Callback<PagedRequest>(req => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(pageNumber, pageSize, searchTerm, sortBy, sortDirection);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.PageNumber.Should().Be(pageNumber);
        capturedRequest.PageSize.Should().Be(pageSize);
        capturedRequest.SearchTerm.Should().Be(searchTerm);
        capturedRequest.SortBy.Should().Be(sortBy);
        capturedRequest.SortDirection.Should().Be(sortDirection);
    }

    [Fact]
    public async Task GetArticlesPaged_WhenServiceThrowsException_ShouldReturnErrorResponse()
    {
        // Arrange
        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetArticlesPaged();

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        
        var apiResponse = actionResult.Value.Should().BeOfType<ApiResponse<PagedResponse<ArticleDto>>>().Subject;
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Contain("Error");
    }

    [Fact]
    public async Task GetArticlesPaged_WithEmptyResult_ShouldReturnEmptyPagedResponse()
    {
        // Arrange
        var emptyResponse = PagedResponse<ArticleDto>.Empty(1, 10);
        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(emptyResponse);

        // Act
        var result = await _controller.GetArticlesPaged();

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = actionResult.Value.Should().BeOfType<ApiResponse<PagedResponse<ArticleDto>>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Items.Should().BeEmpty();
        apiResponse.Data.TotalCount.Should().Be(0);
        apiResponse.Data.TotalPages.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetArticlesPaged_WithNullOrEmptySearchTerm_ShouldPassNullSearchTerm(string? searchTerm)
    {
        // Arrange
        var expectedResponse = CreateTestPagedResponse(10, 1, 10, 25);
        PagedRequest? capturedRequest = null;

        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .Callback<PagedRequest>(req => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(searchTerm: searchTerm);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.SearchTerm.Should().Be(searchTerm);
    }

    [Fact]
    public async Task GetArticlesPaged_ShouldLogPaginationInfo()
    {
        // Arrange
        var expectedResponse = CreateTestPagedResponse(10, 2, 10, 25);
        _mockService.Setup(s => s.GetArticlesPagedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _controller.GetArticlesPaged(2, 10);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved page 2 of articles")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static PagedResponse<ArticleDto> CreateTestPagedResponse(int itemCount, int pageNumber, int pageSize, int totalCount)
    {
        var items = Enumerable.Range(1, itemCount)
            .Select(i => new ArticleDto
            {
                Id = i,
                Name = $"Article {i}",
                SKU = $"SKU{i:D3}",
                Brand = $"Brand {i}",
                Description = $"Description {i}",
                CustomAttributes = new Dictionary<string, object>()
            })
            .ToList();

        return new PagedResponse<ArticleDto>(items, pageNumber, pageSize, totalCount);
    }
}
