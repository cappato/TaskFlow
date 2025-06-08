using Moq;
using TaskFlow.Server.Models;
using TaskFlow.Server.Repositories;
using TaskFlow.Server.Services;
using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;
using FluentAssertions;
using Xunit;

namespace TaskFlow.Server.Tests.Services;

public class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<ICustomAttributeRepository> _mockCustomAttributeRepository;
    private readonly Mock<IArticleAttributeValueRepository> _mockAttributeValueRepository;
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockCustomAttributeRepository = new Mock<ICustomAttributeRepository>();
        _mockAttributeValueRepository = new Mock<IArticleAttributeValueRepository>();
        _service = new ArticleService(_mockArticleRepository.Object, _mockCustomAttributeRepository.Object, _mockAttributeValueRepository.Object);
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
                Brand = "Test Brand",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Category = new Category { Id = 1, Name = "Test Category" },
                Supplier = new User { Id = 1, Name = "Test Supplier" },
                AttributeValues = new List<ArticleAttributeValue>
                {
                    new ArticleAttributeValue
                    {
                        CustomAttribute = new CustomAttribute { Name = "color", DisplayName = "Color" },
                        Value = "Red"
                    }
                }
            }
        };

        _mockArticleRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(articles);

        // Act
        var result = await _service.GetAllArticlesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        var articleDto = result.First();
        articleDto.SKU.Should().Be("TEST-001");
        articleDto.Name.Should().Be("Test Article");
        articleDto.CategoryName.Should().Be("Test Category");
        articleDto.SupplierName.Should().Be("Test Supplier");
        articleDto.CustomAttributes.Should().ContainKey("color");
        articleDto.CustomAttributes["color"].Should().Be("Red");
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

        _mockArticleRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(article);

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
        _mockArticleRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await _service.GetArticleByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateArticleAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "NEW-001",
            Name = "New Article",
            Description = "New Description",
            Type = ArticleType.Clothing,
            Brand = "New Brand",
            CustomAttributes = new Dictionary<string, object>
            {
                { "color", "Blue" },
                { "size", "M" }
            }
        };

        var createdArticle = new Article
        {
            Id = 1,
            SKU = createDto.SKU,
            Name = createDto.Name,
            Description = createDto.Description,
            Type = createDto.Type,
            Brand = createDto.Brand,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync(createDto.SKU))
            .ReturnsAsync(false);

        _mockArticleRepository.Setup(x => x.CreateAsync(It.IsAny<Article>()))
            .ReturnsAsync(createdArticle);

        // Act
        var result = await _service.CreateArticleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.SKU.Should().Be("NEW-001");
        result.Name.Should().Be("New Article");
        result.Type.Should().Be(ArticleType.Clothing);

        _mockArticleRepository.Verify(x => x.ExistsBySKUAsync(createDto.SKU), Times.Once);
        _mockArticleRepository.Verify(x => x.CreateAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task CreateArticleAsync_WithDuplicateSKU_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "EXISTING-001",
            Name = "New Article"
        };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync(createDto.SKU))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateArticleAsync(createDto));

        exception.Message.Should().Contain("Ya existe un artículo con SKU: EXISTING-001");

        _mockArticleRepository.Verify(x => x.CreateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public async Task UpdateArticleAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingArticle = new Article
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Original Name",
            Type = ArticleType.Footwear,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var updateDto = new UpdateArticleDto
        {
            Name = "Updated Name",
            Brand = "Updated Brand",
            IsActive = false
        };

        var updatedArticle = new Article
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Updated Name",
            Brand = "Updated Brand",
            Type = ArticleType.Footwear,
            CreatedAt = existingArticle.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsActive = false
        };

        _mockArticleRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingArticle);

        _mockArticleRepository.Setup(x => x.UpdateAsync(It.IsAny<Article>()))
            .ReturnsAsync(updatedArticle);

        // Act
        var result = await _service.UpdateArticleAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Brand.Should().Be("Updated Brand");
        result.IsActive.Should().BeFalse();

        _mockArticleRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockArticleRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task UpdateArticleAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Name" };

        _mockArticleRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await _service.UpdateArticleAsync(999, updateDto);

        // Assert
        result.Should().BeNull();

        _mockArticleRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public async Task DeleteArticleAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteArticleAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockArticleRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteArticleAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockArticleRepository.Verify(x => x.DeleteAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetArticlesByAttributeAsync_ShouldReturnFilteredArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article
            {
                Id = 1,
                SKU = "TEST-001",
                Name = "Red Shoes",
                Type = ArticleType.Footwear,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        _mockArticleRepository.Setup(x => x.GetByAttributeAsync("color", "red"))
            .ReturnsAsync(articles);

        // Act
        var result = await _service.GetArticlesByAttributeAsync("color", "red");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Red Shoes");

        _mockArticleRepository.Verify(x => x.GetByAttributeAsync("color", "red"), Times.Once);
    }
}
