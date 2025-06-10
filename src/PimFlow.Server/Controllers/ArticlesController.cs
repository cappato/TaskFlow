using Microsoft.AspNetCore.Mvc;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Domain.Enums;
using PimFlow.Server.Controllers.Base;
using PimFlow.Shared.Common;

namespace PimFlow.Server.Controllers;

public class ArticlesController : BaseResourceController<ArticleDto, CreateArticleDto, UpdateArticleDto, IArticleService>
{
    public ArticlesController(IArticleService articleService, ILogger<ArticlesController> logger, IDomainEventService? domainEventService = null)
        : base(articleService, logger, domainEventService)
    {
    }

    // Implementación de métodos abstractos del BaseResourceController
    protected override async Task<IEnumerable<ArticleDto>> GetAllItemsAsync()
    {
        return await Service.GetAllArticlesAsync();
    }

    protected override async Task<ArticleDto?> GetItemByIdAsync(int id)
    {
        return await Service.GetArticleByIdAsync(id);
    }

    protected override async Task<ArticleDto> CreateItemAsync(CreateArticleDto createDto)
    {
        return await Service.CreateArticleAsync(createDto);
    }

    protected override async Task<ArticleDto?> UpdateItemAsync(int id, UpdateArticleDto updateDto)
    {
        return await Service.UpdateArticleAsync(id, updateDto);
    }

    protected override async Task<bool> DeleteItemAsync(int id)
    {
        return await Service.DeleteArticleAsync(id);
    }

    // Endpoints específicos de Articles (no cubiertos por BaseResourceController)

    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetArticleBySKU(string sku)
    {
        var validationResult = ValidateStringParameter<ArticleDto>(sku, nameof(sku));
        if (validationResult != null)
            return validationResult;

        return await ExecuteAsync(async () =>
        {
            var article = await Service.GetArticleBySKUAsync(sku);
            if (article == null)
                throw new InvalidOperationException("Artículo no encontrado");
            return article;
        }, "GetArticleBySKU");
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>> GetArticlesByCategory(int categoryId)
    {
        return await ExecuteAsync(async () =>
        {
            var articles = await Service.GetArticlesByCategoryIdAsync(categoryId);
            Logger.LogInformation("Retrieved {ArticleCount} articles for category {CategoryId}",
                articles?.Count() ?? 0, categoryId);
            return articles;
        }, "GetArticlesByCategory");
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>> GetArticlesByType(ArticleType type)
    {
        return await ExecuteAsync(async () =>
        {
            var articles = await Service.GetArticlesByTypeAsync(type);
            Logger.LogInformation("Retrieved {ArticleCount} articles for type {Type}",
                articles?.Count() ?? 0, type);
            return articles;
        }, "GetArticlesByType");
    }

    [HttpGet("brand/{brand}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>> GetArticlesByBrand(string brand)
    {
        var validationResult = ValidateStringParameter<IEnumerable<ArticleDto>>(brand, nameof(brand));
        if (validationResult != null)
            return validationResult;

        return await ExecuteAsync(async () =>
        {
            var articles = await Service.GetArticlesByBrandAsync(brand);
            Logger.LogInformation("Retrieved {ArticleCount} articles for brand {Brand}",
                articles?.Count() ?? 0, brand);
            return articles;
        }, "GetArticlesByBrand");
    }

    [HttpGet("attribute")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>> GetArticlesByAttribute(
        [FromQuery] string attributeName,
        [FromQuery] string value)
    {
        var attributeValidation = ValidateStringParameter<IEnumerable<ArticleDto>>(attributeName, nameof(attributeName));
        if (attributeValidation != null)
            return attributeValidation;

        var valueValidation = ValidateStringParameter<IEnumerable<ArticleDto>>(value, nameof(value));
        if (valueValidation != null)
            return valueValidation;

        return await ExecuteAsync(async () =>
        {
            var articles = await Service.GetArticlesByAttributeAsync(attributeName, value);
            Logger.LogInformation("Retrieved {ArticleCount} articles for attribute {AttributeName}={Value}",
                articles?.Count() ?? 0, attributeName, value);
            return articles;
        }, "GetArticlesByAttribute");
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>> SearchArticles([FromQuery] string term)
    {
        var validationResult = ValidateStringParameter<IEnumerable<ArticleDto>>(term, "término de búsqueda");
        if (validationResult != null)
            return validationResult;

        return await ExecuteAsync(async () =>
        {
            var articles = await Service.SearchArticlesAsync(term);
            Logger.LogInformation("Search for '{SearchTerm}' returned {ArticleCount} articles",
                term, articles?.Count() ?? 0);
            return articles;
        }, "SearchArticles");
    }

}
