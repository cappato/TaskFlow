using FluentAssertions;
using PimFlow.Domain.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.ValueObjects;

/// <summary>
/// Tests para el Value Object DeletionInfo
/// Verifica que encapsule correctamente la información de eliminación
/// </summary>
public class DeletionInfoTests
{
    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Unit")]
    public void DeletionInfo_WithZeroCounts_ShouldIndicateCanBeDeleted()
    {
        // Arrange & Act
        var deletionInfo = new DeletionInfo(0, 0);
        
        // Assert
        deletionInfo.ActiveSubCategories.Should().Be(0);
        deletionInfo.ActiveArticles.Should().Be(0);
        deletionInfo.CanBeDeleted.Should().BeTrue();
        deletionInfo.Summary.Should().Be("La categoría puede ser eliminada");
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Unit")]
    public void DeletionInfo_WithActiveSubCategories_ShouldIndicateCannotBeDeleted()
    {
        // Arrange & Act
        var deletionInfo = new DeletionInfo(2, 0);
        
        // Assert
        deletionInfo.ActiveSubCategories.Should().Be(2);
        deletionInfo.ActiveArticles.Should().Be(0);
        deletionInfo.CanBeDeleted.Should().BeFalse();
        deletionInfo.Summary.Should().Be("La categoría no puede ser eliminada: 2 subcategorías activas, 0 artículos activos");
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Unit")]
    public void DeletionInfo_WithActiveArticles_ShouldIndicateCannotBeDeleted()
    {
        // Arrange & Act
        var deletionInfo = new DeletionInfo(0, 3);
        
        // Assert
        deletionInfo.ActiveSubCategories.Should().Be(0);
        deletionInfo.ActiveArticles.Should().Be(3);
        deletionInfo.CanBeDeleted.Should().BeFalse();
        deletionInfo.Summary.Should().Be("La categoría no puede ser eliminada: 0 subcategorías activas, 3 artículos activos");
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Unit")]
    public void DeletionInfo_WithBothActiveCounts_ShouldIndicateCannotBeDeleted()
    {
        // Arrange & Act
        var deletionInfo = new DeletionInfo(1, 5);
        
        // Assert
        deletionInfo.ActiveSubCategories.Should().Be(1);
        deletionInfo.ActiveArticles.Should().Be(5);
        deletionInfo.CanBeDeleted.Should().BeFalse();
        deletionInfo.Summary.Should().Be("La categoría no puede ser eliminada: 1 subcategorías activas, 5 artículos activos");
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Equality")]
    public void DeletionInfo_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var deletionInfo1 = new DeletionInfo(2, 3);
        var deletionInfo2 = new DeletionInfo(2, 3);
        
        // Act & Assert
        deletionInfo1.Should().Be(deletionInfo2);
        deletionInfo1.GetHashCode().Should().Be(deletionInfo2.GetHashCode());
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "Equality")]
    public void DeletionInfo_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var deletionInfo1 = new DeletionInfo(2, 3);
        var deletionInfo2 = new DeletionInfo(2, 4);
        var deletionInfo3 = new DeletionInfo(1, 3);
        
        // Act & Assert
        deletionInfo1.Should().NotBe(deletionInfo2);
        deletionInfo1.Should().NotBe(deletionInfo3);
        deletionInfo2.Should().NotBe(deletionInfo3);
    }

    [Fact]
    [Trait("Category", "ValueObject")]
    [Trait("Type", "String")]
    public void ToString_ShouldReturnSummary()
    {
        // Arrange
        var deletionInfo = new DeletionInfo(1, 2);
        
        // Act
        var result = deletionInfo.ToString();
        
        // Assert
        result.Should().Be(deletionInfo.Summary);
        result.Should().Be("La categoría no puede ser eliminada: 1 subcategorías activas, 2 artículos activos");
    }
}
