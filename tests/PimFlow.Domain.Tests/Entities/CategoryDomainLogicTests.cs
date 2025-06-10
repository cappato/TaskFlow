using FluentAssertions;
using PimFlow.Domain.Entities;
using PimFlow.Domain.ValueObjects;
using Xunit;

namespace PimFlow.Domain.Tests.Entities;

/// <summary>
/// Tests para la lógica de negocio de Category en el dominio
/// Verifica que las reglas de negocio estén correctamente implementadas en la entidad
/// </summary>
public class CategoryDomainLogicTests
{
    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void CanBeDeleted_WithNoSubCategoriesOrArticles_ShouldReturnSuccess()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        
        // Act
        var result = category.CanBeDeleted();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CanBeDeleted.Should().BeTrue();
        result.Value.ActiveSubCategories.Should().Be(0);
        result.Value.ActiveArticles.Should().Be(0);
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void CanBeDeleted_WithActiveSubCategories_ShouldReturnFailure()
    {
        // Arrange
        var parentCategory = CreateTestCategory("Parent Category");
        var activeSubCategory = CreateTestCategory("Active Sub");
        activeSubCategory.IsActive = true;
        
        parentCategory.SubCategories.Add(activeSubCategory);
        
        // Act
        var result = parentCategory.CanBeDeleted();
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("1 subcategorías activas");
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void CanBeDeleted_WithActiveArticles_ShouldReturnFailure()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        var activeArticle = CreateTestArticle("Test Article");
        activeArticle.IsActive = true;
        
        category.Articles.Add(activeArticle);
        
        // Act
        var result = category.CanBeDeleted();
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("1 artículos activos");
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void CanBeDeleted_WithInactiveSubCategoriesAndArticles_ShouldReturnSuccess()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        var inactiveSubCategory = CreateTestCategory("Inactive Sub");
        inactiveSubCategory.IsActive = false;
        var inactiveArticle = CreateTestArticle("Inactive Article");
        inactiveArticle.IsActive = false;
        
        category.SubCategories.Add(inactiveSubCategory);
        category.Articles.Add(inactiveArticle);
        
        // Act
        var result = category.CanBeDeleted();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CanBeDeleted.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void MarkAsDeleted_WhenCanBeDeleted_ShouldMarkAsInactive()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        var originalUpdatedAt = category.UpdatedAt;
        
        // Act
        var result = category.MarkAsDeleted();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        category.IsActive.Should().BeFalse();
        category.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void MarkAsDeleted_WhenCannotBeDeleted_ShouldReturnFailure()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        var activeSubCategory = CreateTestCategory("Active Sub");
        activeSubCategory.IsActive = true;
        category.SubCategories.Add(activeSubCategory);
        
        var originalIsActive = category.IsActive;
        
        // Act
        var result = category.MarkAsDeleted();
        
        // Assert
        result.IsFailure.Should().BeTrue();
        category.IsActive.Should().Be(originalIsActive); // Should not change
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void WouldCreateCircularReference_WithDirectCircularReference_ShouldReturnTrue()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        category.Id = 1;
        
        // Act
        var result = category.WouldCreateCircularReference(1, id => null); // Self-reference
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void WouldCreateCircularReference_WithIndirectCircularReference_ShouldReturnTrue()
    {
        // Arrange
        var categoryA = CreateTestCategory("Category A");
        categoryA.Id = 1;
        
        var categoryB = CreateTestCategory("Category B");
        categoryB.Id = 2;
        categoryB.ParentCategoryId = 1;
        
        var categoryC = CreateTestCategory("Category C");
        categoryC.Id = 3;
        categoryC.ParentCategoryId = 2;
        
        // Mock function to simulate repository lookup
        Category? GetCategoryById(int id) => id switch
        {
            1 => categoryA,
            2 => categoryB,
            3 => categoryC,
            _ => null
        };
        
        // Act - Try to make categoryA child of categoryC (would create A -> C -> B -> A)
        var result = categoryA.WouldCreateCircularReference(3, GetCategoryById);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Domain")]
    [Trait("Type", "BusinessLogic")]
    public void WouldCreateCircularReference_WithValidHierarchy_ShouldReturnFalse()
    {
        // Arrange
        var categoryA = CreateTestCategory("Category A");
        categoryA.Id = 1;
        
        var categoryB = CreateTestCategory("Category B");
        categoryB.Id = 2;
        
        // Mock function
        Category? GetCategoryById(int id) => id switch
        {
            1 => categoryA,
            2 => categoryB,
            _ => null
        };
        
        // Act - Valid parent-child relationship
        var result = categoryA.WouldCreateCircularReference(2, GetCategoryById);
        
        // Assert
        result.Should().BeFalse();
    }

    private static Category CreateTestCategory(string name)
    {
        var result = Category.Create(name, "Test description");
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    private static Article CreateTestArticle(string name)
    {
        var result = Article.Create("TEST-SKU", name, "Test Brand", Domain.Enums.ArticleType.Footwear);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }
}
