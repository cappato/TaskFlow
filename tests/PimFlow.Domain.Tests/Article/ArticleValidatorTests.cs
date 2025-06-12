using FluentAssertions;
using PimFlow.Domain.Article;
using PimFlow.Domain.Article.Enums;
using Xunit;

namespace PimFlow.Domain.Tests.Article;

/// <summary>
/// Tests para ArticleValidator - Validaciones específicas de artículos
/// </summary>
public class ArticleValidatorTests
{
    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void ValidateForCreation_ValidData_ShouldReturnSuccess()
    {
        // Arrange
        var sku = "VALID123";
        var name = "Valid Product";
        var brand = "Valid Brand";

        // Act
        var validationResult = ArticleValidator.ValidateForCreation(sku, name, brand);

        // Assert
        validationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void ValidateForCreation_InvalidSKU_ShouldReturnFailure()
    {
        // Arrange
        var sku = ""; // Invalid SKU
        var name = "Valid Product";
        var brand = "Valid Brand";

        // Act
        var validationResult = ArticleValidator.ValidateForCreation(sku, name, brand);

        // Assert
        validationResult.IsFailure.Should().BeTrue();
        validationResult.Error.Should().Contain("SKU");
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void SkuValidator_ValidSKU_ShouldReturnSuccess()
    {
        // Arrange
        var sku = "VALID123";

        // Act
        var validationResult = ArticleValidator.Sku.Validate(sku);

        // Assert
        validationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void SkuValidator_InvalidSKU_ShouldReturnFailure()
    {
        // Arrange
        var sku = "invalid-sku";

        // Act
        var validationResult = ArticleValidator.Sku.Validate(sku);

        // Assert
        validationResult.IsFailure.Should().BeTrue();
        validationResult.Error.Should().Contain("SKU");
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void NameValidator_ValidName_ShouldReturnSuccess()
    {
        // Arrange
        var name = "Valid Product Name";

        // Act
        var validationResult = ArticleValidator.Name.Validate(name);

        // Assert
        validationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void NameValidator_EmptyName_ShouldReturnFailure()
    {
        // Arrange
        var name = "";

        // Act
        var validationResult = ArticleValidator.Name.Validate(name);

        // Assert
        validationResult.IsFailure.Should().BeTrue();
        validationResult.Error.Should().Contain("nombre");
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void BrandValidator_ValidBrand_ShouldReturnSuccess()
    {
        // Arrange
        var brand = "Valid Brand";

        // Act
        var validationResult = ArticleValidator.Brand.Validate(brand);

        // Assert
        validationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "Validation")]
    [Trait("Type", "UnitTest")]
    public void BrandValidator_EmptyBrand_ShouldReturnFailure()
    {
        // Arrange
        var brand = "";

        // Act
        var validationResult = ArticleValidator.Brand.Validate(brand);

        // Assert
        validationResult.IsFailure.Should().BeTrue();
        validationResult.Error.Should().Contain("marca");
    }


}
