using AutoMapper;
using Moq;
using PimFlow.Domain.Article;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Article.Enums;
using PimFlow.Server.Mapping;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Mappers;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Services;

public class ArticleServiceTests
{
    private readonly Mock<IArticleQueryService> _mockQueryService;
    private readonly Mock<IArticleCommandService> _mockCommandService;
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        _mockQueryService = new Mock<IArticleQueryService>();
        _mockCommandService = new Mock<IArticleCommandService>();

        _service = new ArticleService(_mockQueryService.Object, _mockCommandService.Object);
    }

    [Fact]
    public async Task GetAllArticlesAsync_ShouldDelegateToQueryService()
    {
        // Arrange
        var expectedArticles = new List<ArticleDto>
        {
            new ArticleDto { Id = 1, SKU = "TEST-001", Name = "Test Article" }
        };

        _mockQueryService.Setup(x => x.GetAllArticlesAsync())
            .ReturnsAsync(expectedArticles);

        // Act
        var result = await _service.GetAllArticlesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedArticles);
        _mockQueryService.Verify(x => x.GetAllArticlesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldDelegateToQueryService()
    {
        // Arrange
        var expectedArticle = new ArticleDto { Id = 1, SKU = "TEST-001", Name = "Test Article" };

        _mockQueryService.Setup(x => x.GetArticleByIdAsync(1))
            .ReturnsAsync(expectedArticle);

        // Act
        var result = await _service.GetArticleByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedArticle);
        _mockQueryService.Verify(x => x.GetArticleByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateArticleAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        var createDto = new CreateArticleDto { SKU = "NEW-001", Name = "New Article" };
        var expectedArticle = new ArticleDto { Id = 1, SKU = "NEW-001", Name = "New Article" };

        _mockCommandService.Setup(x => x.CreateArticleAsync(createDto))
            .ReturnsAsync(expectedArticle);

        // Act
        var result = await _service.CreateArticleAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedArticle);
        _mockCommandService.Verify(x => x.CreateArticleAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Name" };
        var expectedArticle = new ArticleDto { Id = 1, Name = "Updated Name" };

        _mockCommandService.Setup(x => x.UpdateArticleAsync(1, updateDto))
            .ReturnsAsync(expectedArticle);

        // Act
        var result = await _service.UpdateArticleAsync(1, updateDto);

        // Assert
        result.Should().BeEquivalentTo(expectedArticle);
        _mockCommandService.Verify(x => x.UpdateArticleAsync(1, updateDto), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        _mockCommandService.Setup(x => x.DeleteArticleAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteArticleAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockCommandService.Verify(x => x.DeleteArticleAsync(1), Times.Once);
    }

    // NOTE: Detailed business logic tests moved to specific CQRS service tests
    // This facade only tests delegation behavior
}
