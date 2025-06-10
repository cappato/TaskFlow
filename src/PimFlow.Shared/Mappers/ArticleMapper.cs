using PimFlow.Shared.DTOs;
using PimFlow.Shared.ViewModels;
using PimFlow.Shared.Enums;

namespace PimFlow.Shared.Mappers;

/// <summary>
/// Mapper para convertir entre DTOs y ViewModels de artículos
/// Separa la lógica de mapeo de la lógica de negocio y UI
/// </summary>
public static class ArticleMapper
{
    /// <summary>
    /// Convierte ArticleDto a ArticleViewModel para UI
    /// </summary>
    public static ArticleViewModel ToViewModel(ArticleDto dto)
    {
        return new ArticleViewModel
        {
            Id = dto.Id,
            SKU = dto.SKU,
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type.ToString(),
            Brand = dto.Brand,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            SupplierId = dto.SupplierId,
            SupplierName = dto.SupplierName,
            CustomAttributes = new Dictionary<string, object>(dto.CustomAttributes)
        };
    }

    /// <summary>
    /// Convierte lista de ArticleDto a lista de ArticleViewModel
    /// </summary>
    public static List<ArticleViewModel> ToViewModelList(IEnumerable<ArticleDto> dtos)
    {
        return dtos.Select(ToViewModel).ToList();
    }

    /// <summary>
    /// Convierte ArticleViewModel a CreateArticleDto para API
    /// </summary>
    public static CreateArticleDto ToCreateDto(CreateArticleViewModel viewModel)
    {
        return new CreateArticleDto
        {
            SKU = viewModel.SKU.Trim(),
            Name = viewModel.Name.Trim(),
            Description = viewModel.Description?.Trim() ?? string.Empty,
            Type = Enum.Parse<ArticleType>(viewModel.Type),
            Brand = viewModel.Brand.Trim(),
            CategoryId = viewModel.CategoryId,
            SupplierId = viewModel.SupplierId,
            CustomAttributes = new Dictionary<string, object>(viewModel.CustomAttributes)
        };
    }

    /// <summary>
    /// Convierte ArticleViewModel a UpdateArticleDto para API
    /// </summary>
    public static UpdateArticleDto ToUpdateDto(UpdateArticleViewModel viewModel)
    {
        return new UpdateArticleDto
        {
            SKU = viewModel.SKU.Trim(),
            Name = viewModel.Name.Trim(),
            Description = viewModel.Description?.Trim() ?? string.Empty,
            Type = Enum.Parse<ArticleType>(viewModel.Type),
            Brand = viewModel.Brand.Trim(),
            CategoryId = viewModel.CategoryId,
            SupplierId = viewModel.SupplierId,
            CustomAttributes = new Dictionary<string, object>(viewModel.CustomAttributes)
        };
    }

    /// <summary>
    /// Convierte ArticleDto a CreateArticleViewModel para edición
    /// </summary>
    public static CreateArticleViewModel ToCreateViewModel(ArticleDto? dto = null)
    {
        if (dto == null)
        {
            return new CreateArticleViewModel();
        }

        return new CreateArticleViewModel
        {
            SKU = dto.SKU,
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type.ToString(),
            Brand = dto.Brand,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            CustomAttributes = new Dictionary<string, object>(dto.CustomAttributes)
        };
    }

    /// <summary>
    /// Convierte ArticleDto a UpdateArticleViewModel para edición
    /// </summary>
    public static UpdateArticleViewModel ToUpdateViewModel(ArticleDto dto)
    {
        return new UpdateArticleViewModel
        {
            Id = dto.Id,
            SKU = dto.SKU,
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type.ToString(),
            Brand = dto.Brand,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            CustomAttributes = new Dictionary<string, object>(dto.CustomAttributes),
            HasChanges = false
        };
    }

    /// <summary>
    /// Copia datos de ArticleViewModel a UpdateArticleViewModel
    /// </summary>
    public static void CopyToUpdateViewModel(ArticleViewModel source, UpdateArticleViewModel target)
    {
        target.Id = source.Id;
        target.SKU = source.SKU;
        target.Name = source.Name;
        target.Description = source.Description;
        target.Type = source.Type;
        target.Brand = source.Brand;
        target.CategoryId = source.CategoryId;
        target.SupplierId = source.SupplierId;
        target.CustomAttributes = new Dictionary<string, object>(source.CustomAttributes);
        target.MarkAsChanged();
    }

    /// <summary>
    /// Valida que los datos del ViewModel sean consistentes antes del mapeo
    /// </summary>
    public static bool ValidateForMapping(CreateArticleViewModel viewModel, out List<string> errors)
    {
        var validation = PimFlow.Contracts.Validation.SharedValidationRules.Mapping.ValidateCreateArticle(
            viewModel.SKU, viewModel.Name, viewModel.Brand, viewModel.Type);

        errors = validation.Errors;
        return validation.IsValid;
    }

    /// <summary>
    /// Valida que los datos del ViewModel sean consistentes antes del mapeo
    /// </summary>
    public static bool ValidateForMapping(UpdateArticleViewModel viewModel, out List<string> errors)
    {
        var validation = PimFlow.Contracts.Validation.SharedValidationRules.Mapping.ValidateUpdateArticle(
            viewModel.Id, viewModel.SKU, viewModel.Name, viewModel.Brand, viewModel.Type);

        errors = validation.Errors;
        return validation.IsValid;
    }
}
