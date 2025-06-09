using FluentAssertions;
using PimFlow.Domain.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.ValueObjects;

public class SKUTests
{
    [Fact]
    public void Create_ValidSKU_ShouldSucceed()
    {
        // Arrange
        var validSku = "ABC123";

        // Act
        var result = SKU.Create(validSku);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("ABC123");
    }

    [Fact]
    public void Create_ValidSKUWithLowercase_ShouldNormalizeToUppercase()
    {
        // Arrange
        var skuWithLowercase = "abc123";

        // Act
        var result = SKU.Create(skuWithLowercase);

        // Assert
        result.Value.Should().Be("ABC123");
    }

    [Fact]
    public void Create_ValidSKUWithSpaces_ShouldTrimSpaces()
    {
        // Arrange
        var skuWithSpaces = "  ABC123  ";

        // Act
        var result = SKU.Create(skuWithSpaces);

        // Assert
        result.Value.Should().Be("ABC123");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyOrNullSKU_ShouldThrowException(string invalidSku)
    {
        // Act & Assert
        var action = () => SKU.Create(invalidSku);
        action.Should().Throw<ArgumentException>()
            .WithMessage("SKU no puede estar vacío*");
    }

    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("A")] // Too short
    public void Create_TooShortSKU_ShouldThrowException(string shortSku)
    {
        // Act & Assert
        var action = () => SKU.Create(shortSku);
        action.Should().Throw<ArgumentException>()
            .WithMessage("SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres*");
    }

    [Fact]
    public void Create_TooLongSKU_ShouldThrowException()
    {
        // Arrange
        var longSku = new string('A', 51); // 51 characters

        // Act & Assert
        var action = () => SKU.Create(longSku);
        action.Should().Throw<ArgumentException>()
            .WithMessage("SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres*");
    }

    [Theory]
    [InlineData("ABC-123")] // Contains hyphen (invalid)
    [InlineData("ABC@123")] // Contains special character
    [InlineData("ABC 123")] // Contains space
    [InlineData("ABC.123")] // Contains dot
    public void Create_InvalidCharacters_ShouldThrowException(string invalidSku)
    {
        // Act & Assert
        var action = () => SKU.Create(invalidSku);
        action.Should().Throw<ArgumentException>()
            .WithMessage("SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres*");
    }

    [Theory]
    [InlineData("ABC123", true)]
    [InlineData("XYZ789", true)]
    [InlineData("A1B2C3", true)]
    [InlineData("123ABC", true)]
    [InlineData("AB", false)] // Too short
    [InlineData("ABC-123", false)] // Invalid character
    [InlineData("", false)] // Empty
    [InlineData(null, false)] // Null
    public void IsValid_ShouldReturnCorrectResult(string sku, bool expected)
    {
        // Act
        var result = SKU.IsValid(sku);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("ABC123", true)]
    [InlineData("XYZ789", true)]
    [InlineData("AB", false)]
    [InlineData("ABC-123", false)]
    public void TryCreate_ShouldReturnCorrectResult(string sku, bool shouldSucceed)
    {
        // Act
        var result = SKU.TryCreate(sku, out var createdSku);

        // Assert
        result.Should().Be(shouldSucceed);
        if (shouldSucceed)
        {
            createdSku.Should().NotBeNull();
            createdSku!.Value.Should().Be(sku.ToUpperInvariant());
        }
        else
        {
            createdSku.Should().BeNull();
        }
    }

    [Fact]
    public void Equals_SameSKU_ShouldReturnTrue()
    {
        // Arrange
        var sku1 = SKU.Create("ABC123");
        var sku2 = SKU.Create("ABC123");

        // Act & Assert
        sku1.Should().Be(sku2);
        (sku1 == sku2).Should().BeTrue();
        (sku1 != sku2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentSKU_ShouldReturnFalse()
    {
        // Arrange
        var sku1 = SKU.Create("ABC123");
        var sku2 = SKU.Create("XYZ789");

        // Act & Assert
        sku1.Should().NotBe(sku2);
        (sku1 == sku2).Should().BeFalse();
        (sku1 != sku2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameSKU_ShouldReturnSameHashCode()
    {
        // Arrange
        var sku1 = SKU.Create("ABC123");
        var sku2 = SKU.Create("ABC123");

        // Act & Assert
        sku1.GetHashCode().Should().Be(sku2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var sku = SKU.Create("ABC123");

        // Act
        var result = sku.ToString();

        // Assert
        result.Should().Be("ABC123");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var sku = SKU.Create("ABC123");

        // Act
        string stringValue = sku;

        // Assert
        stringValue.Should().Be("ABC123");
    }
}
