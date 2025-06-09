using FluentAssertions;
using PimFlow.Domain.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.ValueObjects;

public class ProductNameTests
{
    [Theory]
    [InlineData("Nike Air Max")]
    [InlineData("Adidas Ultraboost")]
    [InlineData("Producto-Test")]
    [InlineData("Producto_Test")]
    [InlineData("AB")] // Minimum length
    public void Create_ValidName_ShouldSucceed(string validName)
    {
        // Act
        var result = ProductName.Create(validName);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(validName);
    }

    [Fact]
    public void Create_ValidNameWithSpaces_ShouldTrimSpaces()
    {
        // Arrange
        var nameWithSpaces = "  Nike Air Max  ";

        // Act
        var result = ProductName.Create(nameWithSpaces);

        // Assert
        result.Value.Should().Be("Nike Air Max");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyOrNullName_ShouldThrowException(string invalidName)
    {
        // Act & Assert
        var action = () => ProductName.Create(invalidName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El nombre no puede estar vacío*");
    }

    [Fact]
    public void Create_TooShortName_ShouldThrowException()
    {
        // Arrange
        var shortName = "A"; // 1 character

        // Act & Assert
        var action = () => ProductName.Create(shortName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El nombre debe tener al menos 2 caracteres*");
    }

    [Fact]
    public void Create_TooLongName_ShouldThrowException()
    {
        // Arrange
        var longName = new string('A', 201); // 201 characters

        // Act & Assert
        var action = () => ProductName.Create(longName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El nombre no puede exceder 200 caracteres*");
    }

    [Theory]
    [InlineData("---")]
    [InlineData("___")]
    [InlineData("-_-")]
    public void Create_OnlySpecialCharacters_ShouldThrowException(string invalidName)
    {
        // Act & Assert
        var action = () => ProductName.Create(invalidName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El nombre debe contener caracteres alfanuméricos*");
    }

    [Fact]
    public void Create_OnlySpaces_ShouldThrowEmptyException()
    {
        // Act & Assert
        var action = () => ProductName.Create("   ");
        action.Should().Throw<ArgumentException>()
            .WithMessage("El nombre no puede estar vacío*");
    }

    [Theory]
    [InlineData("Nike Air Max", true)]
    [InlineData("Producto-Test", true)]
    [InlineData("Producto_Test", true)]
    [InlineData("AB", true)] // Minimum valid length
    [InlineData("A", false)] // Too short
    [InlineData("", false)] // Empty
    [InlineData(null, false)] // Null
    [InlineData("---", false)] // Only special characters
    public void IsValid_ShouldReturnCorrectResult(string name, bool expected)
    {
        // Act
        var result = ProductName.IsValid(name);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Nike Air Max", true)]
    [InlineData("AB", true)]
    [InlineData("A", false)]
    [InlineData("---", false)]
    public void TryCreate_ShouldReturnCorrectResult(string name, bool shouldSucceed)
    {
        // Act
        var result = ProductName.TryCreate(name, out var createdName);

        // Assert
        result.Should().Be(shouldSucceed);
        if (shouldSucceed)
        {
            createdName.Should().NotBeNull();
            createdName!.Value.Should().Be(name);
        }
        else
        {
            createdName.Should().BeNull();
        }
    }

    [Fact]
    public void Equals_SameName_ShouldReturnTrue()
    {
        // Arrange
        var name1 = ProductName.Create("Nike Air Max");
        var name2 = ProductName.Create("Nike Air Max");

        // Act & Assert
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();
        (name1 != name2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentName_ShouldReturnFalse()
    {
        // Arrange
        var name1 = ProductName.Create("Nike Air Max");
        var name2 = ProductName.Create("Adidas Ultraboost");

        // Act & Assert
        name1.Should().NotBe(name2);
        (name1 == name2).Should().BeFalse();
        (name1 != name2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameName_ShouldReturnSameHashCode()
    {
        // Arrange
        var name1 = ProductName.Create("Nike Air Max");
        var name2 = ProductName.Create("Nike Air Max");

        // Act & Assert
        name1.GetHashCode().Should().Be(name2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var name = ProductName.Create("Nike Air Max");

        // Act
        var result = name.ToString();

        // Assert
        result.Should().Be("Nike Air Max");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var name = ProductName.Create("Nike Air Max");

        // Act
        string stringValue = name;

        // Assert
        stringValue.Should().Be("Nike Air Max");
    }

    [Fact]
    public void Create_MaxLengthName_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('A', 200); // Exactly 200 characters

        // Act
        var result = ProductName.Create(maxLengthName);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(maxLengthName);
        result.Value.Length.Should().Be(200);
    }
}
