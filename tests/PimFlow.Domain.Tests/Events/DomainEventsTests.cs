using FluentAssertions;
using PimFlow.Domain.Common;
using PimFlow.Domain.Events;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using Xunit;

namespace PimFlow.Domain.Tests.Events;

/// <summary>
/// Tests para la infraestructura de Domain Events
/// Verifica que los eventos se generen y manejen correctamente
/// </summary>
public class DomainEventsTests
{
    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "Infrastructure")]
    public void AggregateRoot_WhenCreated_ShouldHaveNoDomainEvents()
    {
        // Arrange & Act
        var article = CreateTestArticle("Test Article");
        
        // Assert
        article.HasDomainEvents.Should().BeFalse();
        article.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "Infrastructure")]
    public void AggregateRoot_WhenEventAdded_ShouldHaveDomainEvents()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        
        // Act
        article.MarkAsDeleted("Test deletion");
        
        // Assert
        article.HasDomainEvents.Should().BeTrue();
        article.DomainEvents.Should().HaveCount(1);
        article.DomainEvents.First().Should().BeOfType<ArticleDeletedEvent>();
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "Infrastructure")]
    public void AggregateRoot_WhenEventsCleared_ShouldHaveNoDomainEvents()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        article.MarkAsDeleted("Test deletion");
        
        // Act
        article.ClearDomainEvents();
        
        // Assert
        article.HasDomainEvents.Should().BeFalse();
        article.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "ArticleEvents")]
    public void Article_WhenDeleted_ShouldPublishArticleDeletedEvent()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        article.Id = 1;
        
        // Act
        var result = article.MarkAsDeleted("Test reason");
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        article.HasDomainEvents.Should().BeTrue();
        
        var deletedEvent = article.DomainEvents.OfType<ArticleDeletedEvent>().FirstOrDefault();
        deletedEvent.Should().NotBeNull();
        deletedEvent!.ArticleId.Should().Be(1);
        deletedEvent.SKU.Should().Be("TEST-SKU");
        deletedEvent.Name.Should().Be("Test Article");
        deletedEvent.Reason.Should().Be("Test reason");
        deletedEvent.EventName.Should().Be("ArticleDeleted");
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "ArticleEvents")]
    public void Article_WhenCategoryChanged_ShouldPublishCategoryChangedEvent()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        article.Id = 1;
        article.CategoryId = 5;
        
        // Act
        var result = article.ChangeCategoryTo(10);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        article.HasDomainEvents.Should().BeTrue();
        
        var categoryChangedEvent = article.DomainEvents.OfType<ArticleCategoryChangedEvent>().FirstOrDefault();
        categoryChangedEvent.Should().NotBeNull();
        categoryChangedEvent!.ArticleId.Should().Be(1);
        categoryChangedEvent.SKU.Should().Be("TEST-SKU");
        categoryChangedEvent.PreviousCategoryId.Should().Be(5);
        categoryChangedEvent.NewCategoryId.Should().Be(10);
        categoryChangedEvent.EventName.Should().Be("ArticleCategoryChanged");
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "ArticleEvents")]
    public void Article_WhenUpdated_ShouldPublishArticleUpdatedEvent()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        article.Id = 1;
        
        // Act
        var result = article.UpdateWith(name: "Updated Article", description: "Updated Description");
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        article.HasDomainEvents.Should().BeTrue();
        
        var updatedEvent = article.DomainEvents.OfType<ArticleUpdatedEvent>().FirstOrDefault();
        updatedEvent.Should().NotBeNull();
        updatedEvent!.ArticleId.Should().Be(1);
        updatedEvent.SKU.Should().Be("TEST-SKU");
        updatedEvent.Name.Should().Be("Updated Article");
        updatedEvent.ModifiedFields.Should().Contain("Name");
        updatedEvent.ModifiedFields.Should().Contain("Description");
        updatedEvent.EventName.Should().Be("ArticleUpdated");
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "CategoryEvents")]
    public void Category_WhenDeleted_ShouldPublishCategoryDeletedEvent()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        category.Id = 1;
        
        // Act
        var result = category.MarkAsDeleted("Test reason");
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        category.HasDomainEvents.Should().BeTrue();
        
        var deletedEvent = category.DomainEvents.OfType<CategoryDeletedEvent>().FirstOrDefault();
        deletedEvent.Should().NotBeNull();
        deletedEvent!.CategoryId.Should().Be(1);
        deletedEvent.Name.Should().Be("Test Category");
        deletedEvent.Reason.Should().Be("Test reason");
        deletedEvent.EventName.Should().Be("CategoryDeleted");
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "CategoryEvents")]
    public void Category_WhenHierarchyChanged_ShouldPublishHierarchyChangedEvent()
    {
        // Arrange
        var category = CreateTestCategory("Test Category");
        category.Id = 1;
        category.ParentCategoryId = 5;
        
        // Mock function for circular reference check
        Category? GetCategoryById(int id) => null; // No circular reference
        
        // Act
        var result = category.ChangeParentTo(10, GetCategoryById);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        category.HasDomainEvents.Should().BeTrue();
        
        var hierarchyChangedEvent = category.DomainEvents.OfType<CategoryHierarchyChangedEvent>().FirstOrDefault();
        hierarchyChangedEvent.Should().NotBeNull();
        hierarchyChangedEvent!.CategoryId.Should().Be(1);
        hierarchyChangedEvent.Name.Should().Be("Test Category");
        hierarchyChangedEvent.PreviousParentId.Should().Be(5);
        hierarchyChangedEvent.NewParentId.Should().Be(10);
        hierarchyChangedEvent.EventName.Should().Be("CategoryHierarchyChanged");
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "EventProperties")]
    public void DomainEvent_WhenCreated_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var eventId = Guid.NewGuid();
        var occurredOn = DateTime.UtcNow;
        var articleEvent = new ArticleCreatedEvent(1, "TEST-SKU", "Test Article", ArticleType.Footwear, "Test Brand");
        
        // Assert
        articleEvent.EventId.Should().NotBeEmpty();
        articleEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        articleEvent.EventName.Should().Be("ArticleCreated");
        articleEvent.Version.Should().Be(1);
        articleEvent.ToString().Should().Contain("ArticleCreated");
        articleEvent.ToString().Should().Contain(articleEvent.EventId.ToString());
    }

    [Fact]
    [Trait("Category", "DomainEvents")]
    [Trait("Type", "AggregateHelpers")]
    public void AggregateRoot_GetDomainEventsOfType_ShouldReturnCorrectEvents()
    {
        // Arrange
        var article = CreateTestArticle("Test Article");
        article.MarkAsDeleted("Test deletion");
        article.ChangeCategoryTo(5);
        
        // Act
        var deletedEvents = article.GetDomainEventsOfType<ArticleDeletedEvent>().ToList();
        var categoryChangedEvents = article.GetDomainEventsOfType<ArticleCategoryChangedEvent>().ToList();
        var createdEvents = article.GetDomainEventsOfType<ArticleCreatedEvent>().ToList();
        
        // Assert
        deletedEvents.Should().HaveCount(1);
        categoryChangedEvents.Should().HaveCount(1);
        createdEvents.Should().HaveCount(0);
        
        article.HasDomainEventsOfType<ArticleDeletedEvent>().Should().BeTrue();
        article.HasDomainEventsOfType<ArticleCategoryChangedEvent>().Should().BeTrue();
        article.HasDomainEventsOfType<ArticleCreatedEvent>().Should().BeFalse();
    }

    private static Article CreateTestArticle(string name)
    {
        var result = Article.Create("TEST-SKU", name, "Test Brand", ArticleType.Footwear);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    private static Category CreateTestCategory(string name)
    {
        var result = Category.Create(name, "Test description");
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }
}
