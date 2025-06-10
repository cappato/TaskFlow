using AutoMapper;
using FluentAssertions;
using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Mapping;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests for ArticleQueryService (CQRS Query side)
/// Tests the actual business logic for read operations
/// </summary>
public class ArticleQueryServiceTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly ArticleQueryService _service;

    public ArticleQueryServiceTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        
        // Configure real AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });
        _mapper = configuration.CreateMapper();
        
        _service = new ArticleQueryService(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAllArticlesAsync_ShouldReturnMappedArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article
            {
                Id = 1,
                SKU = "TEST-001",
                Name = "Test Article",
                Type = ArticleType.Footwear,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(articles);

        // Act
        var result = await _service.GetAllArticlesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var dto = result.First();
        dto.SKU.Should().Be("TEST-001");
        dto.Name.Should().Be("Test Article");
    }

    [Fact]
    public async Task GetArticleByIdAsync_WithValidId_ShouldReturnMappedArticle()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Test Article",
            Type = ArticleType.Footwear,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(article);

        // Act
        var result = await _service.GetArticleByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetArticleByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.GetArticleByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetArticlesByCategoryIdAsync_ShouldReturnFilteredArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article { Id = 1, SKU = "CAT1-001", CategoryId = 1 },
            new Article { Id = 2, SKU = "CAT1-002", CategoryId = 1 }
        };

        _mockRepository.Setup(x => x.GetByCategoryIdAsync(1)).ReturnsAsync(articles);

        // Act
        var result = await _service.GetArticlesByCategoryIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.All(a => a.SKU.StartsWith("CAT1")).Should().BeTrue();
    }

    [Fact]
    public async Task SearchArticlesAsync_ShouldReturnMatchingArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article { Id = 1, SKU = "SEARCH-001", Name = "Searchable Article" }
        };

        _mockRepository.Setup(x => x.SearchAsync("search")).ReturnsAsync(articles);

        // Act
        var result = await _service.SearchArticlesAsync("search");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Searchable Article");
    }
}
