using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Domain.Article.Enums;
using Xunit;

namespace PimFlow.Server.Tests.ISP;

/// <summary>
/// Tests demonstrating practical usage of Interface Segregation Principle
/// Shows how segregated interfaces improve design and reduce coupling
/// </summary>
public class ISPUsageExamplesTests
{
    [Fact]
    public async Task ReadOnlyService_ShouldOnlyDependOnReaderInterface()
    {
        // Arrange - Service that only needs to read articles
        var mockReader = new Mock<IArticleReader>();
        mockReader.Setup(r => r.GetAllArticlesAsync())
                  .ReturnsAsync(new List<ArticleDto> { new ArticleDto { Id = 1, Name = "Test" } });

        var displayService = new ArticleDisplayService(mockReader.Object);

        // Act
        var result = await displayService.GetArticlesForDisplay();

        // Assert - Service only has access to read operations
        result.Should().NotBeEmpty();
        mockReader.Verify(r => r.GetAllArticlesAsync(), Times.Once);

        // Cannot access write methods - compile-time safety
        // displayService._articleReader.CreateArticleAsync(...); // ❌ Not available
    }

    [Fact]
    public async Task FilterService_ShouldOnlyDependOnFilterInterface()
    {
        // Arrange - Service that only needs filtering capabilities
        var mockFilter = new Mock<IArticleFilter>();
        mockFilter.Setup(f => f.GetArticlesByCategoryIdAsync(1))
                  .ReturnsAsync(new List<ArticleDto> { new ArticleDto { Id = 1, CategoryId = 1 } });

        var filterService = new ArticleFilterService(mockFilter.Object);

        // Act
        var count = await filterService.CountArticlesByCategory(1);

        // Assert - Service only has access to filter operations
        count.Should().Be(1);
        mockFilter.Verify(f => f.GetArticlesByCategoryIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task ImportService_ShouldOnlyDependOnWriterInterface()
    {
        // Arrange - Service that only needs to create articles
        var mockWriter = new Mock<IArticleWriter>();
        var testDto = new CreateArticleDto { SKU = "TEST", Name = "Test Article" };
        mockWriter.Setup(w => w.CreateArticleAsync(It.IsAny<CreateArticleDto>()))
                  .ReturnsAsync(new ArticleDto { Id = 1, SKU = "TEST", Name = "Test Article" });

        var importService = new ArticleImportService(mockWriter.Object);

        // Act
        var result = await importService.ImportSingleArticle(testDto);

        // Assert - Service only has access to write operations
        result.Should().NotBeNull();
        result.SKU.Should().Be("TEST");
        mockWriter.Verify(w => w.CreateArticleAsync(It.IsAny<CreateArticleDto>()), Times.Once);
    }

    [Fact]
    public async Task CategoryNavigationService_ShouldOnlyDependOnHierarchyInterface()
    {
        // Arrange - Service that only needs hierarchy navigation
        var mockHierarchy = new Mock<ICategoryHierarchy>();
        mockHierarchy.Setup(h => h.GetRootCategoriesAsync())
                     .ReturnsAsync(new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Root" } });

        var navigationService = new CategoryNavigationService(mockHierarchy.Object);

        // Act
        var menu = await navigationService.GenerateNavigationMenu();

        // Assert - Service only has access to hierarchy operations
        menu.Should().NotBeEmpty();
        mockHierarchy.Verify(h => h.GetRootCategoriesAsync(), Times.Once);
    }

    [Fact]
    public void DependencyInjection_ShouldResolveSpecificInterfaces()
    {
        // Arrange - Test that DI can resolve specific interfaces
        var services = new ServiceCollection();
        
        // Mock implementations
        var mockQueryService = new Mock<IArticleQueryService>();
        var mockCommandService = new Mock<IArticleCommandService>();
        
        services.AddSingleton(mockQueryService.Object);
        services.AddSingleton(mockCommandService.Object);
        services.AddSingleton<ArticleService>();
        
        // Register segregated interfaces
        services.AddSingleton<IArticleReader>(provider => provider.GetRequiredService<ArticleService>());
        services.AddSingleton<IArticleWriter>(provider => provider.GetRequiredService<ArticleService>());
        services.AddSingleton<IArticleFilter>(provider => provider.GetRequiredService<ArticleService>());

        var serviceProvider = services.BuildServiceProvider();

        // Act - Resolve specific interfaces
        var reader = serviceProvider.GetService<IArticleReader>();
        var writer = serviceProvider.GetService<IArticleWriter>();
        var filter = serviceProvider.GetService<IArticleFilter>();

        // Assert - All specific interfaces should be available
        reader.Should().NotBeNull();
        writer.Should().NotBeNull();
        filter.Should().NotBeNull();
        
        // All should resolve to the same instance (ArticleService)
        reader.Should().BeSameAs(writer);
        writer.Should().BeSameAs(filter);
    }

    [Fact]
    public void InterfaceSizes_ShouldBeAppropriatelySmall()
    {
        // Arrange - Get method counts for segregated interfaces
        var readerMethods = typeof(IArticleReader).GetMethods().Length;
        var writerMethods = typeof(IArticleWriter).GetMethods().Length;
        var filterMethods = typeof(IArticleFilter).GetMethods().Length;
        var hierarchyMethods = typeof(ICategoryHierarchy).GetMethods().Length;

        // Assert - Interfaces should be small and focused
        readerMethods.Should().BeLessOrEqualTo(10, "Reader interface should be small");
        writerMethods.Should().BeLessOrEqualTo(8, "Writer interface should be small");
        filterMethods.Should().BeLessOrEqualTo(8, "Filter interface should be small");
        hierarchyMethods.Should().BeLessOrEqualTo(5, "Hierarchy interface should be small");
    }

    [Fact]
    public void FacadeInterfaces_ShouldInheritFromSegregatedInterfaces()
    {
        // Arrange - Check facade interface inheritance
        var articleServiceInterfaces = typeof(IArticleService).GetInterfaces();
        var categoryServiceInterfaces = typeof(ICategoryService).GetInterfaces();
        var customAttributeServiceInterfaces = typeof(ICustomAttributeService).GetInterfaces();

        // Assert - Facades should inherit from segregated interfaces
        articleServiceInterfaces.Should().Contain(typeof(IArticleReader));
        articleServiceInterfaces.Should().Contain(typeof(IArticleWriter));
        articleServiceInterfaces.Should().Contain(typeof(IArticleFilter));

        categoryServiceInterfaces.Should().Contain(typeof(ICategoryReader));
        categoryServiceInterfaces.Should().Contain(typeof(ICategoryHierarchy));
        categoryServiceInterfaces.Should().Contain(typeof(ICategoryWriter));

        customAttributeServiceInterfaces.Should().Contain(typeof(ICustomAttributeReader));
        customAttributeServiceInterfaces.Should().Contain(typeof(ICustomAttributeWriter));
    }
}

// Example services demonstrating ISP benefits
public class ArticleDisplayService
{
    private readonly IArticleReader _articleReader;

    public ArticleDisplayService(IArticleReader articleReader)
    {
        _articleReader = articleReader;
    }

    public async Task<IEnumerable<string>> GetArticlesForDisplay()
    {
        var articles = await _articleReader.GetAllArticlesAsync();
        return articles.Select(a => $"{a.Name} - {a.SKU}");
    }
}

public class ArticleFilterService
{
    private readonly IArticleFilter _articleFilter;

    public ArticleFilterService(IArticleFilter articleFilter)
    {
        _articleFilter = articleFilter;
    }

    public async Task<int> CountArticlesByCategory(int categoryId)
    {
        var articles = await _articleFilter.GetArticlesByCategoryIdAsync(categoryId);
        return articles.Count();
    }
}

public class ArticleImportService
{
    private readonly IArticleWriter _articleWriter;

    public ArticleImportService(IArticleWriter articleWriter)
    {
        _articleWriter = articleWriter;
    }

    public async Task<ArticleDto> ImportSingleArticle(CreateArticleDto createDto)
    {
        return await _articleWriter.CreateArticleAsync(createDto);
    }
}

public class CategoryNavigationService
{
    private readonly ICategoryHierarchy _categoryHierarchy;

    public CategoryNavigationService(ICategoryHierarchy categoryHierarchy)
    {
        _categoryHierarchy = categoryHierarchy;
    }

    public async Task<string> GenerateNavigationMenu()
    {
        var rootCategories = await _categoryHierarchy.GetRootCategoriesAsync();
        return string.Join(", ", rootCategories.Select(c => c.Name));
    }
}
