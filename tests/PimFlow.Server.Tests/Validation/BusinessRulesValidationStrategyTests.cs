using FluentAssertions;
using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Validation;

/// <summary>
/// Tests for BusinessRulesValidationStrategy
/// Tests business logic validation rules
/// </summary>
public class BusinessRulesValidationStrategyTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly BusinessRulesValidationStrategy _strategy;

    public BusinessRulesValidationStrategyTests()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _strategy = new BusinessRulesValidationStrategy(_mockArticleRepository.Object, _mockCategoryRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_WithUniqueSKUAndValidCategory_ShouldPass()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "UNIQUE-001",
            Name = "Test Article",
            CategoryId = 1
        };

        var category = new Category { Id = 1, Name = "Test Category", IsActive = true };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("UNIQUE-001")).ReturnsAsync(false);
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithDuplicateSKU_ShouldFail()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "DUPLICATE-001",
            Name = "Test Article",
            CategoryId = 1
        };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("DUPLICATE-001")).ReturnsAsync(true);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Ya existe un artículo con SKU: DUPLICATE-001");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidCategory_ShouldFail()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "UNIQUE-001",
            Name = "Test Article",
            CategoryId = 999
        };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("UNIQUE-001")).ReturnsAsync(false);
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("La categoría con ID 999 no existe");
    }

    [Fact]
    public async Task ValidateAsync_WithInactiveCategory_ShouldWarn()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "UNIQUE-001",
            Name = "Test Article",
            CategoryId = 1
        };

        var inactiveCategory = new Category { Id = 1, Name = "Inactive Category", IsActive = false };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("UNIQUE-001")).ReturnsAsync(false);
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inactiveCategory);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue(); // Warnings don't make it invalid
        result.Warnings.Should().Contain("La categoría 'Inactive Category' está inactiva");
    }

    [Fact]
    public async Task ValidateAsync_WithoutCategory_ShouldFail()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "UNIQUE-001",
            Name = "Test Article"
            // CategoryId not set
        };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("UNIQUE-001")).ReturnsAsync(false);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Categoría es requerida");
    }

    [Fact]
    public async Task ValidateAsync_WithTooManyCustomAttributes_ShouldWarn()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            SKU = "UNIQUE-001",
            Name = "Test Article",
            CategoryId = 1,
            CustomAttributes = Enumerable.Range(1, 25)
                .ToDictionary(i => $"attr{i}", i => (object)$"value{i}")
        };

        var category = new Category { Id = 1, Name = "Test Category", IsActive = true };

        _mockArticleRepository.Setup(x => x.ExistsBySKUAsync("UNIQUE-001")).ReturnsAsync(false);
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue(); // Warnings don't make it invalid
        result.Warnings.Should().Contain(w => w.Contains("muchos atributos personalizados"));
    }

    [Fact]
    public void Strategy_ShouldHaveCorrectPriorityAndCategory()
    {
        // Assert
        _strategy.Priority.Should().Be(2);
        _strategy.Name.Should().Be("Business Rules Validation");
        _strategy.Category.Should().Be(ValidationCategory.BusinessRules);
    }
}
