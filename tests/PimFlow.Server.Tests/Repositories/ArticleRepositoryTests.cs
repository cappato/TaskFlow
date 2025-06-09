using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Server.Repositories;
using FluentAssertions;
using Xunit;

namespace TaskFlow.Server.Tests.Repositories;

public class ArticleRepositoryTests : IDisposable
{
    private readonly TaskFlowDbContext _context;
    private readonly ArticleRepository _repository;

    public ArticleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskFlowDbContext(options);
        _repository = new ArticleRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var supplier = new User
        {
            Id = 1,
            Name = "Test Supplier",
            Email = "supplier@test.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var customAttribute = new CustomAttribute
        {
            Id = 1,
            Name = "color",
            DisplayName = "Color",
            Type = AttributeType.Text,
            IsRequired = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var article = new Article
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Test Article",
            Description = "Test Description",
            Type = ArticleType.Footwear,
            Brand = "Test Brand",
            CategoryId = 1,
            SupplierId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var attributeValue = new ArticleAttributeValue
        {
            Id = 1,
            ArticleId = 1,
            CustomAttributeId = 1,
            Value = "Red",
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        _context.Users.Add(supplier);
        _context.CustomAttributes.Add(customAttribute);
        _context.Articles.Add(article);
        _context.ArticleAttributeValues.Add(attributeValue);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllArticles()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnArticle()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.SKU.Should().Be("TEST-001");
        result.Name.Should().Be("Test Article");
        result.Category.Should().NotBeNull();
        result.Supplier.Should().NotBeNull();
        result.AttributeValues.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySKUAsync_WithValidSKU_ShouldReturnArticle()
    {
        // Act
        var result = await _repository.GetBySKUAsync("TEST-001");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Article");
    }

    [Fact]
    public async Task GetBySKUAsync_WithInvalidSKU_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetBySKUAsync("INVALID-SKU");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByAttributeAsync_WithValidAttribute_ShouldReturnArticles()
    {
        // Act
        var result = await _repository.GetByAttributeAsync("color", "Red");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetByAttributeAsync_WithInvalidAttribute_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.GetByAttributeAsync("color", "Blue");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithValidArticle_ShouldCreateSuccessfully()
    {
        // Arrange
        var newArticle = new Article
        {
            SKU = "TEST-002",
            Name = "New Test Article",
            Description = "New Description",
            Type = ArticleType.Clothing,
            Brand = "New Brand",
            CategoryId = 1,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(newArticle);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.SKU.Should().Be("TEST-002");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExistsBySKUAsync_WithExistingSKU_ShouldReturnTrue()
    {
        // Act
        var result = await _repository.ExistsBySKUAsync("TEST-001");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsBySKUAsync_WithNonExistingSKU_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsBySKUAsync("NON-EXISTING");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteSuccessfully()
    {
        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();

        var deletedArticle = await _repository.GetByIdAsync(1);
        deletedArticle.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SearchAsync_WithValidTerm_ShouldReturnMatchingArticles()
    {
        // Act
        var result = await _repository.SearchAsync("Test");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Test");
    }

    [Fact]
    public async Task GetByTypeAsync_WithValidType_ShouldReturnArticles()
    {
        // Act
        var result = await _repository.GetByTypeAsync(ArticleType.Footwear);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Type.Should().Be(ArticleType.Footwear);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
