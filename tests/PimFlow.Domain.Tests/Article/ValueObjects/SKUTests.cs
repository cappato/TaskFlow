using FluentAssertions;
using PimFlow.Domain.Article.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.Article.ValueObjects;

/// <summary>
/// Tests para SKU Value Object - Validaciones de código de producto
/// </summary>
public class SKUTests
{
    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void Create_WithValidSKU_ShouldReturnSuccess()
    {
        // Arrange
        var skuValue = "ABC123XYZ";

        // Act
        var sku = SKU.Create(skuValue);

        // Assert
        sku.Should().NotBeNull();
        sku.Value.Should().Be(skuValue);
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void Create_WithEmptySKU_ShouldThrowException()
    {
        // Arrange
        var skuValue = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SKU.Create(skuValue));
        exception.Message.Should().Contain("SKU");
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void TryCreate_WithValidSKU_ShouldReturnTrue()
    {
        // Arrange
        var skuValue = "VALID123";

        // Act
        var success = SKU.TryCreate(skuValue, out var sku);

        // Assert
        success.Should().BeTrue();
        sku.Should().NotBeNull();
        sku!.Value.Should().Be(skuValue);
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void TryCreate_WithInvalidSKU_ShouldReturnFalse()
    {
        // Arrange
        var skuValue = "invalid-sku";

        // Act
        var success = SKU.TryCreate(skuValue, out var sku);

        // Assert
        success.Should().BeFalse();
        sku.Should().BeNull();
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void IsValid_WithValidSKU_ShouldReturnTrue()
    {
        // Arrange
        var skuValue = "VALID123";

        // Act
        var isValid = SKU.IsValid(skuValue);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void IsValid_WithInvalidSKU_ShouldReturnFalse()
    {
        // Arrange
        var skuValue = "invalid-sku";

        // Act
        var isValid = SKU.IsValid(skuValue);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void Equals_WithSameSKU_ShouldReturnTrue()
    {
        // Arrange
        var sku1 = SKU.Create("ABC123");
        var sku2 = SKU.Create("ABC123");

        // Act & Assert
        sku1.Should().Be(sku2);
        sku1.Equals(sku2).Should().BeTrue();
        (sku1 == sku2).Should().BeTrue();
    }

    [Fact]
    [Trait("Article", "ValueObjects")]
    [Trait("Type", "UnitTest")]
    public void ToString_ShouldReturnSKUValue()
    {
        // Arrange
        var skuValue = "ABC123";
        var sku = SKU.Create(skuValue);

        // Act
        var result = sku.ToString();

        // Assert
        result.Should().Be(skuValue);
    }




}
