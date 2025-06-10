using AutoMapper;
using FluentAssertions;
using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests para CategoryService verificando que use correctamente la lógica del dominio
/// </summary>
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new CategoryService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task DeleteCategoryAsync_WhenCategoryCanBeDeleted_ShouldUseMarkAsDeletedFromDomain()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateTestCategory("Test Category");
        category.Id = categoryId;
        
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);

        // Act
        var result = await _service.DeleteCategoryAsync(categoryId);

        // Assert
        result.Should().BeTrue();
        category.IsActive.Should().BeFalse(); // Domain logic should have marked it as deleted
        _mockRepository.Verify(r => r.UpdateAsync(category), Times.Once);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task DeleteCategoryAsync_WhenCategoryHasActiveSubCategories_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateTestCategory("Parent Category");
        category.Id = categoryId;
        
        var activeSubCategory = CreateTestCategory("Active Sub");
        activeSubCategory.IsActive = true;
        category.SubCategories.Add(activeSubCategory);
        
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeleteCategoryAsync(categoryId));
        
        exception.Message.Should().Contain("subcategorías activas");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task DeleteCategoryAsync_WhenCategoryHasActiveArticles_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateTestCategory("Test Category");
        category.Id = categoryId;
        
        var activeArticle = CreateTestArticle("Active Article");
        activeArticle.IsActive = true;
        category.Articles.Add(activeArticle);
        
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeleteCategoryAsync(categoryId));
        
        exception.Message.Should().Contain("artículos activos");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task DeleteCategoryAsync_WhenCategoryNotFound_ShouldReturnFalse()
    {
        // Arrange
        var categoryId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.DeleteCategoryAsync(categoryId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task UpdateCategoryAsync_WhenCircularReferenceDetected_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoryId = 1;
        var parentId = 2;
        
        var category = CreateTestCategory("Category A");
        category.Id = categoryId;
        
        var parentCategory = CreateTestCategory("Category B");
        parentCategory.Id = parentId;
        parentCategory.ParentCategoryId = categoryId; // This would create a circular reference
        
        var updateDto = new UpdateCategoryDto
        {
            Name = "Updated Category",
            ParentCategoryId = parentId
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _mockRepository.Setup(r => r.GetByIdAsync(parentId))
            .ReturnsAsync(parentCategory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateCategoryAsync(categoryId, updateDto));
        
        exception.Message.Should().Contain("referencia circular");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Type", "DomainLogic")]
    public async Task UpdateCategoryAsync_WithValidParent_ShouldUpdateSuccessfully()
    {
        // Arrange
        var categoryId = 1;
        var parentId = 2;
        
        var category = CreateTestCategory("Category A");
        category.Id = categoryId;
        
        var parentCategory = CreateTestCategory("Category B");
        parentCategory.Id = parentId;
        
        var updateDto = new UpdateCategoryDto
        {
            Name = "Updated Category",
            Description = "Updated Description",
            ParentCategoryId = parentId
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _mockRepository.Setup(r => r.GetByIdAsync(parentId))
            .ReturnsAsync(parentCategory);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);

        // Act
        var result = await _service.UpdateCategoryAsync(categoryId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Category");
        result.Description.Should().Be("Updated Description");
        result.ParentCategoryId.Should().Be(parentId);
        _mockRepository.Verify(r => r.UpdateAsync(category), Times.Once);
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
