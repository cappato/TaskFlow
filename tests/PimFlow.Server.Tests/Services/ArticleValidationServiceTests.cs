using FluentAssertions;
using Moq;
using PimFlow.Server.Services;
using PimFlow.Server.Validation;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests for ArticleValidationService using Strategy Pattern
/// Tests coordination of validation pipeline
/// </summary>
public class ArticleValidationServiceTests
{
    private readonly Mock<IValidationPipeline<CreateArticleDto>> _mockCreatePipeline;
    private readonly Mock<IValidationPipeline<(int Id, UpdateArticleDto Dto)>> _mockUpdatePipeline;
    private readonly ArticleValidationService _service;

    public ArticleValidationServiceTests()
    {
        _mockCreatePipeline = new Mock<IValidationPipeline<CreateArticleDto>>();
        _mockUpdatePipeline = new Mock<IValidationPipeline<(int Id, UpdateArticleDto Dto)>>();
        _service = new ArticleValidationService(_mockCreatePipeline.Object, _mockUpdatePipeline.Object);
    }

    [Fact]
    public async Task ValidateCreateAsync_ShouldDelegateToPipeline()
    {
        // Arrange
        var createDto = new CreateArticleDto { SKU = "TEST-001", Name = "Test Article" };
        var expectedResult = ValidationResult.Success();
        
        _mockCreatePipeline.Setup(x => x.ValidateAsync(createDto)).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateCreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockCreatePipeline.Verify(x => x.ValidateAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task ValidateUpdateAsync_ShouldDelegateToPipeline()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Name" };
        var expectedResult = ValidationResult.Success();
        
        _mockUpdatePipeline.Setup(x => x.ValidateAsync(It.Is<(int Id, UpdateArticleDto Dto)>(t => t.Id == 1 && t.Dto == updateDto))).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateUpdateAsync(1, updateDto);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockUpdatePipeline.Verify(x => x.ValidateAsync(It.Is<(int Id, UpdateArticleDto Dto)>(t => t.Id == 1 && t.Dto == updateDto)), Times.Once);
    }

    [Fact]
    public async Task ValidateCreateAsync_WithValidationErrors_ShouldReturnErrors()
    {
        // Arrange
        var createDto = new CreateArticleDto { SKU = "", Name = "" };
        var expectedResult = ValidationResult.Failure("SKU es requerido", "Nombre es requerido");
        
        _mockCreatePipeline.Setup(x => x.ValidateAsync(createDto)).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateCreateAsync(createDto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("SKU es requerido");
        result.Errors.Should().Contain("Nombre es requerido");
    }

    [Fact]
    public async Task ValidateUpdateAsync_WithValidationErrors_ShouldReturnErrors()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { SKU = "DUPLICATE-SKU" };
        var expectedResult = ValidationResult.Failure("Ya existe un artículo con SKU: DUPLICATE-SKU");
        
        _mockUpdatePipeline.Setup(x => x.ValidateAsync(It.Is<(int Id, UpdateArticleDto Dto)>(t => t.Id == 1 && t.Dto == updateDto))).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ValidateUpdateAsync(1, updateDto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Ya existe un artículo con SKU: DUPLICATE-SKU");
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WithValidSKU_ShouldReturnTrue()
    {
        // Arrange
        var sku = "UNIQUE-SKU";
        var successResult = ValidationResult.Success();
        
        _mockCreatePipeline.Setup(x => x.ValidateAsync(It.IsAny<CreateArticleDto>())).ReturnsAsync(successResult);

        // Act
        var result = await _service.IsSkuUniqueAsync(sku);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSkuUniqueAsync_WithDuplicateSKU_ShouldReturnFalse()
    {
        // Arrange
        var sku = "DUPLICATE-SKU";
        var failureResult = ValidationResult.Failure("Ya existe un artículo con SKU: DUPLICATE-SKU");
        
        _mockCreatePipeline.Setup(x => x.ValidateAsync(It.IsAny<CreateArticleDto>())).ReturnsAsync(failureResult);

        // Act
        var result = await _service.IsSkuUniqueAsync(sku);

        // Assert
        result.Should().BeFalse();
    }
}
