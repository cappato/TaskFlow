using FluentAssertions;
using PimFlow.Shared.Mappers;
using SharedEnums = PimFlow.Shared.Enums;
using Xunit;

namespace PimFlow.Shared.Tests.Mappers;

/// <summary>
/// Tests para EnumMapper de Shared - solo métodos que no requieren Domain
/// Los tests que requieren Domain se han movido a PimFlow.Server.Tests
/// </summary>
public class EnumMapperTests
{
    [Fact]
    public void GetAllArticleTypesForUI_ShouldReturnAllTypes()
    {
        // Act
        var result = EnumMapper.GetAllArticleTypesForUI();

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
        var result = EnumMapper.GetAllAttributeTypesForUI();

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
}
