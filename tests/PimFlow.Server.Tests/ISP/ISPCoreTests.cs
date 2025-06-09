using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PimFlow.Server.Services;
using System.Reflection;
using Xunit;

namespace PimFlow.Server.Tests.ISP;

/// <summary>
/// Essential tests to validate Interface Segregation Principle (ISP) implementation
/// Focuses on core ISP principles without being overly specific about implementation details
/// </summary>
public class ISPCoreTests
{
    [Fact]
    public void SegregatedInterfaces_ShouldBeSmallAndFocused()
    {
        // Arrange - Core ISP principle: interfaces should be small and focused
        var readerMethods = typeof(IArticleReader).GetMethods();
        var writerMethods = typeof(IArticleWriter).GetMethods();
        var filterMethods = typeof(IArticleFilter).GetMethods();

        // Act & Assert - Verify interfaces are appropriately sized
        readerMethods.Should().HaveCountLessThanOrEqualTo(10, "Reader interfaces should be focused");
        writerMethods.Should().HaveCountLessThanOrEqualTo(8, "Writer interfaces should be focused");
        filterMethods.Should().HaveCountLessThanOrEqualTo(8, "Filter interfaces should be focused");

        // Verify no overlap between read and write operations
        var readerMethodNames = readerMethods.Select(m => m.Name).ToList();
        var writerMethodNames = writerMethods.Select(m => m.Name).ToList();

        readerMethodNames.Should().NotIntersectWith(writerMethodNames, 
            "Reader and Writer interfaces should not have overlapping methods");
    }

    [Fact]
    public void FacadeInterfaces_ShouldInheritFromSegregatedInterfaces()
    {
        // Arrange
        var articleServiceInterface = typeof(IArticleService);
        var categoryServiceInterface = typeof(ICategoryService);

        // Act
        var articleServiceInterfaces = articleServiceInterface.GetInterfaces();
        var categoryServiceInterfaces = categoryServiceInterface.GetInterfaces();

        // Assert - Facades should inherit from segregated interfaces
        articleServiceInterfaces.Should().Contain(typeof(IArticleReader), "ArticleService should inherit from IArticleReader");
        articleServiceInterfaces.Should().Contain(typeof(IArticleWriter), "ArticleService should inherit from IArticleWriter");
        articleServiceInterfaces.Should().Contain(typeof(IArticleFilter), "ArticleService should inherit from IArticleFilter");

        categoryServiceInterfaces.Should().Contain(typeof(ICategoryReader), "CategoryService should inherit from ICategoryReader");
        categoryServiceInterfaces.Should().Contain(typeof(ICategoryHierarchy), "CategoryService should inherit from ICategoryHierarchy");
    }

    [Fact]
    public void ServiceImplementations_ShouldImplementSegregatedInterfaces()
    {
        // Arrange
        var articleService = typeof(ArticleService);

        // Act
        var implementedInterfaces = articleService.GetInterfaces();

        // Assert - Service should implement all segregated interfaces
        implementedInterfaces.Should().Contain(typeof(IArticleService), "Should implement facade interface");
        implementedInterfaces.Should().Contain(typeof(IArticleReader), "Should implement reader interface");
        implementedInterfaces.Should().Contain(typeof(IArticleWriter), "Should implement writer interface");
        implementedInterfaces.Should().Contain(typeof(IArticleFilter), "Should implement filter interface");
    }

    [Fact]
    public void DependencyInjection_ShouldRegisterSegregatedInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Add required dependencies (mocked)
        services.AddScoped<IArticleQueryService, MockArticleQueryService>();
        services.AddScoped<IArticleCommandService, MockArticleCommandService>();
        
        // Simulate the registration from Program.cs
        services.AddScoped<ArticleService>();
        services.AddScoped<IArticleService>(provider => provider.GetRequiredService<ArticleService>());
        services.AddScoped<IArticleReader>(provider => provider.GetRequiredService<ArticleService>());
        services.AddScoped<IArticleWriter>(provider => provider.GetRequiredService<ArticleService>());
        services.AddScoped<IArticleFilter>(provider => provider.GetRequiredService<ArticleService>());

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var readerService = serviceProvider.GetService<IArticleReader>();
        var writerService = serviceProvider.GetService<IArticleWriter>();
        var filterService = serviceProvider.GetService<IArticleFilter>();
        var facadeService = serviceProvider.GetService<IArticleService>();

        readerService.Should().NotBeNull("IArticleReader should be registered");
        writerService.Should().NotBeNull("IArticleWriter should be registered");
        filterService.Should().NotBeNull("IArticleFilter should be registered");
        facadeService.Should().NotBeNull("IArticleService facade should be registered");

        // All should resolve to the same instance (scoped behavior)
        readerService.Should().BeSameAs(writerService, "All segregated interfaces should resolve to same instance");
        writerService.Should().BeSameAs(filterService, "All segregated interfaces should resolve to same instance");
        filterService.Should().BeSameAs(facadeService, "All segregated interfaces should resolve to same instance");
    }
}

// Mock implementations for testing
internal class MockArticleQueryService : IArticleQueryService
{
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> GetAllArticlesAsync() => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
    public Task<PimFlow.Shared.DTOs.ArticleDto?> GetArticleByIdAsync(int id) => Task.FromResult<PimFlow.Shared.DTOs.ArticleDto?>(null);
    public Task<PimFlow.Shared.DTOs.ArticleDto?> GetArticleBySKUAsync(string sku) => Task.FromResult<PimFlow.Shared.DTOs.ArticleDto?>(null);
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId) => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> GetArticlesByTypeAsync(PimFlow.Domain.Enums.ArticleType type) => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> GetArticlesByBrandAsync(string brand) => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value) => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
    public Task<IEnumerable<PimFlow.Shared.DTOs.ArticleDto>> SearchArticlesAsync(string searchTerm) => Task.FromResult(Enumerable.Empty<PimFlow.Shared.DTOs.ArticleDto>());
}

internal class MockArticleCommandService : IArticleCommandService
{
    public Task<PimFlow.Shared.DTOs.ArticleDto> CreateArticleAsync(PimFlow.Shared.DTOs.CreateArticleDto createArticleDto) => Task.FromResult(new PimFlow.Shared.DTOs.ArticleDto());
    public Task<PimFlow.Shared.DTOs.ArticleDto?> UpdateArticleAsync(int id, PimFlow.Shared.DTOs.UpdateArticleDto updateArticleDto) => Task.FromResult<PimFlow.Shared.DTOs.ArticleDto?>(null);
    public Task<bool> DeleteArticleAsync(int id) => Task.FromResult(false);
}
