namespace PimFlow.Server.Services.Examples;

/// <summary>
/// Examples demonstrating Interface Segregation Principle (ISP) benefits
/// Shows how segregated interfaces reduce coupling and improve design
/// </summary>

/// <summary>
/// Example: Read-only service that only needs to display articles
/// BEFORE ISP: Had to depend on IArticleService (including write methods)
/// AFTER ISP: Only depends on IArticleReader (no unnecessary dependencies)
/// </summary>
public class ArticleDisplayService
{
    private readonly IArticleReader _articleReader; // ✅ ISP - Only depends on what it needs

    public ArticleDisplayService(IArticleReader articleReader)
    {
        _articleReader = articleReader; // No access to write methods - safer design
    }

    public async Task<string> GenerateArticleListHtml()
    {
        var articles = await _articleReader.GetAllArticlesAsync();
        // This service cannot accidentally call write methods
        // _articleReader.CreateArticleAsync() - ❌ Not available (ISP benefit)
        
        return string.Join("<br>", articles.Select(a => $"<div>{a.Name}</div>"));
    }
}

/// <summary>
/// Example: Filtering service that only needs filtering capabilities
/// BEFORE ISP: Had to depend on full IArticleService
/// AFTER ISP: Only depends on IArticleFilter
/// </summary>
public class ArticleFilterService
{
    private readonly IArticleFilter _articleFilter; // ✅ ISP - Specific interface

    public ArticleFilterService(IArticleFilter articleFilter)
    {
        _articleFilter = articleFilter;
    }

    public async Task<int> CountArticlesByCategory(int categoryId)
    {
        var articles = await _articleFilter.GetArticlesByCategoryIdAsync(categoryId);
        return articles.Count();
        // Cannot access read-all or write methods - focused responsibility
    }
}

/// <summary>
/// Example: Import service that only needs to create articles
/// BEFORE ISP: Had to depend on full IArticleService (including read methods)
/// AFTER ISP: Only depends on IArticleWriter
/// </summary>
public class ArticleImportService
{
    private readonly IArticleWriter _articleWriter; // ✅ ISP - Only write operations

    public ArticleImportService(IArticleWriter articleWriter)
    {
        _articleWriter = articleWriter;
    }

    public async Task ImportArticlesFromCsv(string csvData)
    {
        // Parse CSV and create articles
        // This service cannot accidentally read articles - focused on writing
        // _articleWriter.GetAllArticlesAsync() - ❌ Not available (ISP benefit)

        // Only has access to write operations it actually needs
        // await _articleWriter.CreateArticleAsync(dto);

        await Task.CompletedTask; // Placeholder for actual implementation
    }
}

/// <summary>
/// Example: Category navigation component
/// BEFORE ISP: Had to depend on full ICategoryService
/// AFTER ISP: Only depends on ICategoryHierarchy
/// </summary>
public class CategoryNavigationService
{
    private readonly ICategoryHierarchy _categoryHierarchy; // ✅ ISP - Specific interface

    public CategoryNavigationService(ICategoryHierarchy categoryHierarchy)
    {
        _categoryHierarchy = categoryHierarchy;
    }

    public async Task<string> GenerateNavigationMenu()
    {
        var rootCategories = await _categoryHierarchy.GetRootCategoriesAsync();
        // Cannot access CRUD operations - only navigation methods
        return "Navigation HTML";
    }
}

/// <summary>
/// Example: Admin service that needs full access (uses facade)
/// This shows that facade interfaces are still useful for complex scenarios
/// </summary>
public class ArticleAdminService
{
    private readonly IArticleService _articleService; // ✅ Uses facade when full access needed

    public ArticleAdminService(IArticleService articleService)
    {
        _articleService = articleService; // Full access when legitimately needed
    }

    public async Task PerformComplexAdminOperation()
    {
        // Can use all operations because admin legitimately needs them
        var articles = await _articleService.GetAllArticlesAsync();
        var filtered = await _articleService.GetArticlesByCategoryIdAsync(1);
        // await _articleService.CreateArticleAsync(dto);
    }
}

/// <summary>
/// Example: Demonstration of ISP violation vs compliance
/// </summary>
public static class ISPComparison
{
    /// <summary>
    /// ❌ BEFORE ISP - Violation example
    /// Client forced to depend on methods it doesn't use
    /// </summary>
    public class BadArticleDisplayService
    {
        private readonly IArticleService _articleService; // ❌ Depends on write methods too

        public BadArticleDisplayService(IArticleService articleService)
        {
            _articleService = articleService;
            // This service only displays articles but has access to:
            // - CreateArticleAsync (doesn't need)
            // - UpdateArticleAsync (doesn't need)
            // - DeleteArticleAsync (doesn't need)
            // - All filtering methods (doesn't need)
        }

        // Risk: Could accidentally call write methods
        // Testing: Harder to mock (need to mock all methods)
        // Coupling: High coupling to unnecessary dependencies
    }

    /// <summary>
    /// ✅ AFTER ISP - Compliant example
    /// Client only depends on what it actually uses
    /// </summary>
    public class GoodArticleDisplayService
    {
        private readonly IArticleReader _articleReader; // ✅ Only depends on read operations

        public GoodArticleDisplayService(IArticleReader articleReader)
        {
            _articleReader = articleReader;
            // This service only has access to:
            // - GetAllArticlesAsync (needs)
            // - GetArticleByIdAsync (needs)
            // - GetArticleBySKUAsync (needs)
            // - SearchArticlesAsync (needs)
        }

        // Benefits:
        // - Cannot accidentally call write methods
        // - Easier to test (smaller interface to mock)
        // - Lower coupling
        // - Clear intent and responsibility
    }
}
