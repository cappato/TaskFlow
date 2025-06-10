using Microsoft.AspNetCore.Mvc;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Controllers.Base;
using PimFlow.Shared.Common;

namespace PimFlow.Server.Controllers;

public class CategoriesController : BaseResourceController<CategoryDto, CreateCategoryDto, UpdateCategoryDto, ICategoryService>
{
    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger, IDomainEventService? domainEventService = null)
        : base(categoryService, logger, domainEventService)
    {
    }

    // Implementación de métodos abstractos del BaseResourceController
    protected override async Task<IEnumerable<CategoryDto>> GetAllItemsAsync()
    {
        return await Service.GetAllCategoriesAsync();
    }

    protected override async Task<CategoryDto?> GetItemByIdAsync(int id)
    {
        return await Service.GetCategoryByIdAsync(id);
    }

    protected override async Task<CategoryDto> CreateItemAsync(CreateCategoryDto createDto)
    {
        return await Service.CreateCategoryAsync(createDto);
    }

    protected override async Task<CategoryDto?> UpdateItemAsync(int id, UpdateCategoryDto updateDto)
    {
        return await Service.UpdateCategoryAsync(id, updateDto);
    }

    protected override async Task<bool> DeleteItemAsync(int id)
    {
        return await Service.DeleteCategoryAsync(id);
    }

    // Endpoints específicos de Categories (no cubiertos por BaseResourceController)

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetActiveCategories()
    {
        return await ExecuteAsync(async () =>
        {
            var categories = await Service.GetActiveCategoriesAsync();
            Logger.LogInformation("Retrieved {CategoryCount} active categories", categories?.Count() ?? 0);
            return categories;
        }, "GetActiveCategories");
    }

    [HttpGet("root")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetRootCategories()
    {
        return await ExecuteAsync(async () =>
        {
            var categories = await Service.GetRootCategoriesAsync();
            Logger.LogInformation("Retrieved {CategoryCount} root categories", categories?.Count() ?? 0);
            return categories;
        }, "GetRootCategories");
    }

    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetSubCategories(int id)
    {
        return await ExecuteAsync(async () =>
        {
            var categories = await Service.GetSubCategoriesAsync(id);
            Logger.LogInformation("Retrieved {CategoryCount} subcategories for category {CategoryId}",
                categories?.Count() ?? 0, id);
            return categories;
        }, "GetSubCategories");
    }

}
