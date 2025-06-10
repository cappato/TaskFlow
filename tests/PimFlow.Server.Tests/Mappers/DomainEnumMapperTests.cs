using FluentAssertions;
using PimFlow.Server.Mappers;
using DomainEnums = PimFlow.Domain.Enums;
using SharedEnums = PimFlow.Shared.Enums;
using Xunit;

namespace PimFlow.Server.Tests.Mappers;

/// <summary>
/// Tests para DomainEnumMapper - métodos que requieren Domain
/// Movidos desde PimFlow.Shared.Tests para mantener Clean Architecture
/// </summary>
public class DomainEnumMapperTests
{
    [Theory]
    [InlineData(DomainEnums.ArticleType.Footwear, SharedEnums.ArticleType.Footwear)]
    [InlineData(DomainEnums.ArticleType.Clothing, SharedEnums.ArticleType.Clothing)]
    [InlineData(DomainEnums.ArticleType.Accessory, SharedEnums.ArticleType.Accessory)]
    public void ToShared_ArticleType_ShouldMapCorrectly(DomainEnums.ArticleType domainType, SharedEnums.ArticleType expectedSharedType)
    {
        // Act
        var result = DomainEnumMapper.ToShared(domainType);

        // Assert
        result.Should().Be(expectedSharedType);
    }

    [Theory]
    [InlineData(SharedEnums.ArticleType.Footwear, DomainEnums.ArticleType.Footwear)]
    [InlineData(SharedEnums.ArticleType.Clothing, DomainEnums.ArticleType.Clothing)]
    [InlineData(SharedEnums.ArticleType.Accessory, DomainEnums.ArticleType.Accessory)]
    public void ToDomain_ArticleType_ShouldMapCorrectly(SharedEnums.ArticleType sharedType, DomainEnums.ArticleType expectedDomainType)
    {
        // Act
        var result = DomainEnumMapper.ToDomain(sharedType);

        // Assert
        result.Should().Be(expectedDomainType);
    }

    [Theory]
    [InlineData(DomainEnums.AttributeType.Text, SharedEnums.AttributeType.Text)]
    [InlineData(DomainEnums.AttributeType.Number, SharedEnums.AttributeType.Number)]
    [InlineData(DomainEnums.AttributeType.Integer, SharedEnums.AttributeType.Integer)]
    [InlineData(DomainEnums.AttributeType.Boolean, SharedEnums.AttributeType.Boolean)]
    [InlineData(DomainEnums.AttributeType.Date, SharedEnums.AttributeType.Date)]
    [InlineData(DomainEnums.AttributeType.DateTime, SharedEnums.AttributeType.DateTime)]
    [InlineData(DomainEnums.AttributeType.Select, SharedEnums.AttributeType.Select)]
    [InlineData(DomainEnums.AttributeType.MultiSelect, SharedEnums.AttributeType.MultiSelect)]
    [InlineData(DomainEnums.AttributeType.Color, SharedEnums.AttributeType.Color)]
    [InlineData(DomainEnums.AttributeType.Url, SharedEnums.AttributeType.Url)]
    [InlineData(DomainEnums.AttributeType.Email, SharedEnums.AttributeType.Email)]
    public void ToShared_AttributeType_ShouldMapCorrectly(DomainEnums.AttributeType domainType, SharedEnums.AttributeType expectedSharedType)
    {
        // Act
        var result = DomainEnumMapper.ToShared(domainType);

        // Assert
        result.Should().Be(expectedSharedType);
    }

    [Theory]
    [InlineData(SharedEnums.AttributeType.Text, DomainEnums.AttributeType.Text)]
    [InlineData(SharedEnums.AttributeType.Number, DomainEnums.AttributeType.Number)]
    [InlineData(SharedEnums.AttributeType.Integer, DomainEnums.AttributeType.Integer)]
    [InlineData(SharedEnums.AttributeType.Boolean, DomainEnums.AttributeType.Boolean)]
    [InlineData(SharedEnums.AttributeType.Date, DomainEnums.AttributeType.Date)]
    [InlineData(SharedEnums.AttributeType.DateTime, DomainEnums.AttributeType.DateTime)]
    [InlineData(SharedEnums.AttributeType.Select, DomainEnums.AttributeType.Select)]
    [InlineData(SharedEnums.AttributeType.MultiSelect, DomainEnums.AttributeType.MultiSelect)]
    [InlineData(SharedEnums.AttributeType.Color, DomainEnums.AttributeType.Color)]
    [InlineData(SharedEnums.AttributeType.Url, DomainEnums.AttributeType.Url)]
    [InlineData(SharedEnums.AttributeType.Email, DomainEnums.AttributeType.Email)]
    public void ToDomain_AttributeType_ShouldMapCorrectly(SharedEnums.AttributeType sharedType, DomainEnums.AttributeType expectedDomainType)
    {
        // Act
        var result = DomainEnumMapper.ToDomain(sharedType);

        // Assert
        result.Should().Be(expectedDomainType);
    }

    [Theory]
    [InlineData("Footwear", DomainEnums.ArticleType.Footwear)]
    [InlineData("Clothing", DomainEnums.ArticleType.Clothing)]
    [InlineData("Accessory", DomainEnums.ArticleType.Accessory)]
    [InlineData("footwear", DomainEnums.ArticleType.Footwear)] // Case insensitive
    public void ParseArticleTypeToDomain_ValidString_ShouldReturnCorrectType(string value, DomainEnums.ArticleType expected)
    {
        // Act
        var result = DomainEnumMapper.ParseArticleTypeToDomain(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("InvalidType")]
    public void ParseArticleTypeToDomain_InvalidString_ShouldReturnNull(string value)
    {
        // Act
        var result = DomainEnumMapper.ParseArticleTypeToDomain(value);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("Text", DomainEnums.AttributeType.Text)]
    [InlineData("Number", DomainEnums.AttributeType.Number)]
    [InlineData("Boolean", DomainEnums.AttributeType.Boolean)]
    [InlineData("text", DomainEnums.AttributeType.Text)] // Case insensitive
    public void ParseAttributeTypeToDomain_ValidString_ShouldReturnCorrectType(string value, DomainEnums.AttributeType expected)
    {
        // Act
        var result = DomainEnumMapper.ParseAttributeTypeToDomain(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("InvalidType")]
    public void ParseAttributeTypeToDomain_InvalidString_ShouldReturnNull(string value)
    {
        // Act
        var result = DomainEnumMapper.ParseAttributeTypeToDomain(value);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetArticleTypeDisplayName_ShouldReturnCorrectDisplayName()
    {
        // Arrange
        var domainType = DomainEnums.ArticleType.Footwear;

        // Act
        var result = DomainEnumMapper.GetArticleTypeDisplayName(domainType);

        // Assert
        result.Should().Be("Calzado");
    }

    [Fact]
    public void GetAttributeTypeDisplayName_ShouldReturnCorrectDisplayName()
    {
        // Arrange
        var domainType = DomainEnums.AttributeType.Text;

        // Act
        var result = DomainEnumMapper.GetAttributeTypeDisplayName(domainType);

        // Assert
        result.Should().Be("Texto");
    }

    [Fact]
    public void GetAllArticleTypesForUI_ShouldReturnAllTypes()
    {
        // Act
        var result = DomainEnumMapper.GetAllArticleTypesForUI();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(item => item.Value == "Footwear" && item.DisplayName == "Calzado");
        result.Should().Contain(item => item.Value == "Clothing" && item.DisplayName == "Ropa");
        result.Should().Contain(item => item.Value == "Accessory" && item.DisplayName == "Accesorios");
    }

    [Fact]
    public void GetAllAttributeTypesForUI_ShouldReturnAllTypes()
    {
        // Act
        var result = DomainEnumMapper.GetAllAttributeTypesForUI();

        // Assert
        result.Should().HaveCount(11); // All AttributeType values
        result.Should().Contain(item => item.Value == "Text" && item.DisplayName == "Texto");
        result.Should().Contain(item => item.Value == "Number" && item.DisplayName == "Número Decimal");
        result.Should().Contain(item => item.Value == "Boolean" && item.DisplayName == "Verdadero/Falso");
        
        // Verify all items have required properties
        result.Should().OnlyContain(item => 
            !string.IsNullOrEmpty(item.Value) && 
            !string.IsNullOrEmpty(item.DisplayName) && 
            !string.IsNullOrEmpty(item.Icon));
    }

    [Fact]
    public void ArticleType_RoundTripConversion_ShouldPreserveValue()
    {
        // Arrange
        var originalDomainType = DomainEnums.ArticleType.Clothing;

        // Act
        var sharedType = DomainEnumMapper.ToShared(originalDomainType);
        var backToDomainType = DomainEnumMapper.ToDomain(sharedType);

        // Assert
        backToDomainType.Should().Be(originalDomainType);
    }

    [Fact]
    public void AttributeType_RoundTripConversion_ShouldPreserveValue()
    {
        // Arrange
        var originalDomainType = DomainEnums.AttributeType.MultiSelect;

        // Act
        var sharedType = DomainEnumMapper.ToShared(originalDomainType);
        var backToDomainType = DomainEnumMapper.ToDomain(sharedType);

        // Assert
        backToDomainType.Should().Be(originalDomainType);
    }
}
