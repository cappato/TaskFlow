using FluentAssertions;
using PimFlow.Domain.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    [InlineData("user123@test-domain.com")]
    [InlineData("a@b.co")]
    public void Create_ValidEmail_ShouldSucceed(string validEmail)
    {
        // Act
        var result = Email.Create(validEmail);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Fact]
    public void Create_ValidEmailWithUppercase_ShouldNormalizeToLowercase()
    {
        // Arrange
        var emailWithUppercase = "TEST@EXAMPLE.COM";

        // Act
        var result = Email.Create(emailWithUppercase);

        // Assert
        result.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_ValidEmailWithSpaces_ShouldTrimSpaces()
    {
        // Arrange
        var emailWithSpaces = "  test@example.com  ";

        // Act
        var result = Email.Create(emailWithSpaces);

        // Assert
        result.Value.Should().Be("test@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyOrNullEmail_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        var action = () => Email.Create(invalidEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Email no puede estar vacío*");
    }

    [Fact]
    public void Create_TooLongEmail_ShouldThrowException()
    {
        // Arrange
        var longEmail = new string('a', 190) + "@example.com"; // Over 200 characters

        // Act & Assert
        var action = () => Email.Create(longEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Email no puede exceder 200 caracteres*");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    [InlineData("test@.com")]
    [InlineData("test@example.")]
    [InlineData("test@@example.com")]
    [InlineData("test @example.com")]
    [InlineData("test@exam ple.com")]
    public void Create_InvalidEmailFormat_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        var action = () => Email.Create(invalidEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Formato de email inválido*");
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("user+tag@example.org", true)]
    [InlineData("a@b.co", true)]
    [InlineData("invalid-email", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValid_ShouldReturnCorrectResult(string email, bool expected)
    {
        // Act
        var result = Email.IsValid(email);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("invalid-email", false)]
    [InlineData("@example.com", false)]
    public void TryCreate_ShouldReturnCorrectResult(string email, bool shouldSucceed)
    {
        // Act
        var result = Email.TryCreate(email, out var createdEmail);

        // Assert
        result.Should().Be(shouldSucceed);
        if (shouldSucceed)
        {
            createdEmail.Should().NotBeNull();
            createdEmail!.Value.Should().Be(email.ToLowerInvariant());
        }
        else
        {
            createdEmail.Should().BeNull();
        }
    }

    [Fact]
    public void Equals_SameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM"); // Different case

        // Act & Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("other@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameEmail_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM");

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string stringValue = email;

        // Assert
        stringValue.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_MaxLengthEmail_ShouldSucceed()
    {
        // Arrange
        var maxLengthEmail = new string('a', 191) + "@test.com"; // Exactly 200 characters (191 + 9 = 200)

        // Act
        var result = Email.Create(maxLengthEmail);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(maxLengthEmail.ToLowerInvariant());
        result.Value.Length.Should().Be(200);
    }
}
