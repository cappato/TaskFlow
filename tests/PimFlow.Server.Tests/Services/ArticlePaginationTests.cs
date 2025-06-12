using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Domain.Article;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;
using Xunit;

namespace PimFlow.Server.Tests.Services;

public class ArticlePaginationTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ArticleQueryService _service;

    public ArticlePaginationTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ArticleQueryService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetArticlesPagedAsync_WithValidRequest_ShouldReturnPagedResponse()
    {
        // Arrange
        var articles = CreateTestArticles(25);
        var request = new PagedRequest { PageNumber = 2, PageSize = 10 };
        var expectedDtos = CreateTestArticleDtos(10);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(10);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetArticlesPagedAsync_WithSearchTerm_ShouldFilterResults()
    {
        // Arrange
        var articles = new List<Article>
        {
            CreateArticle("Nike Air Max", "NIKE001", "Nike", "Running shoes"),
            CreateArticle("Adidas Ultraboost", "ADIDAS001", "Adidas", "Running shoes"),
            CreateArticle("Nike Air Force", "NIKE002", "Nike", "Basketball shoes"),
            CreateArticle("Puma Suede", "PUMA001", "Puma", "Casual shoes")
        };

        var request = new PagedRequest { PageNumber = 1, PageSize = 10, SearchTerm = "Nike" };
        var expectedDtos = CreateTestArticleDtos(2);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("sku", "asc")]
    [InlineData("brand", "desc")]
    [InlineData("createdat", "asc")]
    public async Task GetArticlesPagedAsync_WithSorting_ShouldApplyCorrectOrder(string sortBy, string sortDirection)
    {
        // Arrange
        var articles = new List<Article>
        {
            CreateArticle("Zebra Product", "Z001", "Zebra", "Description", DateTime.Now.AddDays(-1)),
            CreateArticle("Alpha Product", "A001", "Alpha", "Description", DateTime.Now.AddDays(-2)),
            CreateArticle("Beta Product", "B001", "Beta", "Description", DateTime.Now)
        };

        var request = new PagedRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            SortBy = sortBy, 
            SortDirection = sortDirection 
        };

        var expectedDtos = CreateTestArticleDtos(3);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetArticlesPagedAsync_WithEmptyRepository_ShouldReturnEmptyResponse()
    {
        // Arrange
        var articles = new List<Article>();
        var request = new PagedRequest { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(new List<ArticleDto>());

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetArticlesPagedAsync_WithLastPage_ShouldReturnPartialResults()
    {
        // Arrange
        var articles = CreateTestArticles(23);
        var request = new PagedRequest { PageNumber = 3, PageSize = 10 };
        var expectedDtos = CreateTestArticleDtos(3);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.PageNumber.Should().Be(3);
        result.TotalCount.Should().Be(23);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    [Theory]
    [InlineData("nike", 2)]
    [InlineData("NIKE", 2)]
    [InlineData("running", 2)]
    [InlineData("basketball", 1)]
    [InlineData("nonexistent", 0)]
    public async Task GetArticlesPagedAsync_SearchTermCaseInsensitive_ShouldFilterCorrectly(string searchTerm, int expectedCount)
    {
        // Arrange
        var articles = new List<Article>
        {
            CreateArticle("Nike Air Max", "NIKE001", "Nike", "Running shoes"),
            CreateArticle("Nike Air Force", "NIKE002", "Nike", "Basketball shoes"),
            CreateArticle("Adidas Ultraboost", "ADIDAS001", "Adidas", "Running shoes"),
            CreateArticle("Puma Suede", "PUMA001", "Puma", "Casual shoes")
        };

        var request = new PagedRequest { PageNumber = 1, PageSize = 10, SearchTerm = searchTerm };
        var expectedDtos = CreateTestArticleDtos(expectedCount);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(expectedCount);
    }

    [Fact]
    public async Task GetArticlesPagedAsync_WithNullSearchTerm_ShouldReturnAllResults()
    {
        // Arrange
        var articles = CreateTestArticles(15);
        var request = new PagedRequest { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var expectedDtos = CreateTestArticleDtos(10);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetArticlesPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(15);
        result.Items.Should().HaveCount(10);
    }

    private static List<Article> CreateTestArticles(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateArticle($"Article {i}", $"SKU{i:D3}", $"Brand{i % 3 + 1}", $"Description {i}"))
            .ToList();
    }

    private static Article CreateArticle(string name, string sku, string brand, string description, DateTime? createdAt = null)
    {
        return new Article
        {
            Id = Random.Shared.Next(1, 1000),
            Name = name,
            SKU = sku,
            Brand = brand,
            Description = description,
            CreatedAt = createdAt ?? DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }

    private static List<ArticleDto> CreateTestArticleDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new ArticleDto
            {
                Id = i,
                Name = $"Article {i}",
                SKU = $"SKU{i:D3}",
                Brand = $"Brand{i % 3 + 1}",
                Description = $"Description {i}",
                CustomAttributes = new Dictionary<string, object>()
            })
            .ToList();
    }
}
