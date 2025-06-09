using FluentAssertions;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using PimFlow.Domain.Specifications;
using Xunit;

namespace PimFlow.Domain.Tests.Specifications;

public class ArticleSpecificationsTests
{
    private Article CreateValidArticle()
    {
        return new Article
        {
            Id = 1,
            SKU = "ABC123",
            Name = "Test Product",
            Brand = "Test Brand",
            Type = ArticleType.Footwear,
            Description = "Test Description",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public void ActiveArticleSpecification_ActiveArticle_ShouldPass()
    {
        // Arrange
        var article = CreateValidArticle();
        article.IsActive = true;
        var spec = new ArticleSpecifications.ActiveArticleSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ActiveArticleSpecification_InactiveArticle_ShouldFail()
    {
        // Arrange
        var article = CreateValidArticle();
        article.IsActive = false;
        var spec = new ArticleSpecifications.ActiveArticleSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("El artículo debe estar activo");
    }

    [Fact]
    public void CompleteArticleDataSpecification_ValidArticle_ShouldPass()
    {
        // Arrange
        var article = CreateValidArticle();
        var spec = new ArticleSpecifications.CompleteArticleDataSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Test Product", "Test Brand")] // Empty SKU
    [InlineData("ABC123", "", "Test Brand")] // Empty Name
    [InlineData("ABC123", "Test Product", "")] // Empty Brand
    [InlineData("", "", "")] // All empty
    public void CompleteArticleDataSpecification_IncompleteData_ShouldFail(string sku, string name, string brand)
    {
        // Arrange
        var article = CreateValidArticle();
        article.SKU = sku;
        article.Name = name;
        article.Brand = brand;
        var spec = new ArticleSpecifications.CompleteArticleDataSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("El artículo debe tener SKU, nombre y marca");
    }

    [Theory]
    [InlineData("ABC123")] // Valid
    [InlineData("XYZ789")] // Valid
    [InlineData("A1B2C3")] // Valid
    public void ValidSKUFormatSpecification_ValidSKU_ShouldPass(string sku)
    {
        // Arrange
        var article = CreateValidArticle();
        article.SKU = sku;
        var spec = new ArticleSpecifications.ValidSKUFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("ABC-123")] // Invalid character
    [InlineData("")] // Empty
    [InlineData("ABC@123")] // Invalid character
    public void ValidSKUFormatSpecification_InvalidSKU_ShouldFail(string sku)
    {
        // Arrange
        var article = CreateValidArticle();
        article.SKU = sku;
        var spec = new ArticleSpecifications.ValidSKUFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("El SKU debe tener un formato válido (3-50 caracteres alfanuméricos)");
    }

    [Theory]
    [InlineData("Test Product")] // Valid
    [InlineData("Nike Air Max")] // Valid
    [InlineData("AB")] // Minimum length
    public void ValidNameFormatSpecification_ValidName_ShouldPass(string name)
    {
        // Arrange
        var article = CreateValidArticle();
        article.Name = name;
        var spec = new ArticleSpecifications.ValidNameFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("A")] // Too short
    [InlineData("")] // Empty
    public void ValidNameFormatSpecification_InvalidName_ShouldFail(string name)
    {
        // Arrange
        var article = CreateValidArticle();
        article.Name = name;
        var spec = new ArticleSpecifications.ValidNameFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("El nombre debe tener un formato válido (2-200 caracteres)");
    }

    [Theory]
    [InlineData("Nike")] // Valid
    [InlineData("Adidas")] // Valid
    [InlineData("AB")] // Minimum length
    public void ValidBrandFormatSpecification_ValidBrand_ShouldPass(string brand)
    {
        // Arrange
        var article = CreateValidArticle();
        article.Brand = brand;
        var spec = new ArticleSpecifications.ValidBrandFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("A")] // Too short
    [InlineData("")] // Empty
    public void ValidBrandFormatSpecification_InvalidBrand_ShouldFail(string brand)
    {
        // Arrange
        var article = CreateValidArticle();
        article.Brand = brand;
        var spec = new ArticleSpecifications.ValidBrandFormatSpecification();

        // Act
        var result = spec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("La marca debe tener un formato válido (2-100 caracteres)");
    }

    [Fact]
    public async Task UniqueSKUSpecification_UniqueSKU_ShouldPass()
    {
        // Arrange
        var article = CreateValidArticle();
        var skuExistsFunc = new Func<string, Task<bool>>(_ => Task.FromResult(false)); // SKU doesn't exist
        var spec = new ArticleSpecifications.UniqueSKUSpecification(skuExistsFunc);

        // Act
        var result = await spec.IsSatisfiedByAsync(article);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UniqueSKUSpecification_DuplicateSKU_ShouldFail()
    {
        // Arrange
        var article = CreateValidArticle();
        var skuExistsFunc = new Func<string, Task<bool>>(_ => Task.FromResult(true)); // SKU exists
        var spec = new ArticleSpecifications.UniqueSKUSpecification(skuExistsFunc);

        // Act
        var result = await spec.IsSatisfiedByAsync(article);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Be("El SKU debe ser único en el sistema");
    }

    [Fact]
    public async Task UniqueSKUSpecification_DuplicateSKUButSameId_ShouldPass()
    {
        // Arrange
        var article = CreateValidArticle();
        article.Id = 1;
        var skuExistsFunc = new Func<string, Task<bool>>(_ => Task.FromResult(true)); // SKU exists
        var spec = new ArticleSpecifications.UniqueSKUSpecification(skuExistsFunc, excludeId: 1); // Exclude same ID

        // Act
        var result = await spec.IsSatisfiedByAsync(article);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SpecificationCombination_AndOperator_ShouldWork()
    {
        // Arrange
        var article = CreateValidArticle();
        var activeSpec = new ArticleSpecifications.ActiveArticleSpecification();
        var completeSpec = new ArticleSpecifications.CompleteArticleDataSpecification();
        var combinedSpec = activeSpec.And(completeSpec);

        // Act
        var result = combinedSpec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SpecificationCombination_AndOperatorWithFailure_ShouldFail()
    {
        // Arrange
        var article = CreateValidArticle();
        article.IsActive = false; // Make it fail the active spec
        var activeSpec = new ArticleSpecifications.ActiveArticleSpecification();
        var completeSpec = new ArticleSpecifications.CompleteArticleDataSpecification();
        var combinedSpec = activeSpec.And(completeSpec);

        // Act
        var result = combinedSpec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeFalse();
        combinedSpec.ErrorMessage.Should().Contain("El artículo debe estar activo");
    }

    [Fact]
    public void SpecificationCombination_OrOperator_ShouldWork()
    {
        // Arrange
        var article = CreateValidArticle();
        article.IsActive = false; // Fails active spec
        var activeSpec = new ArticleSpecifications.ActiveArticleSpecification();
        var completeSpec = new ArticleSpecifications.CompleteArticleDataSpecification(); // This should pass
        var combinedSpec = activeSpec.Or(completeSpec);

        // Act
        var result = combinedSpec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue(); // Should pass because complete spec passes
    }

    [Fact]
    public void SpecificationCombination_NotOperator_ShouldWork()
    {
        // Arrange
        var article = CreateValidArticle();
        article.IsActive = false;
        var activeSpec = new ArticleSpecifications.ActiveArticleSpecification();
        var notActiveSpec = activeSpec.Not();

        // Act
        var result = notActiveSpec.IsSatisfiedBy(article);

        // Assert
        result.Should().BeTrue(); // Should pass because article is NOT active
    }
}
