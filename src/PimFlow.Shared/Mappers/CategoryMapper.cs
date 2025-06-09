using PimFlow.Shared.DTOs;
using PimFlow.Shared.ViewModels;

namespace PimFlow.Shared.Mappers;

/// <summary>
/// Mapper para convertir entre DTOs y ViewModels de categorías
/// Separa la lógica de mapeo de la lógica de negocio y UI
/// </summary>
public static class CategoryMapper
{
    /// <summary>
    /// Convierte CategoryDto a CategoryViewModel para UI
    /// </summary>
    public static CategoryViewModel ToViewModel(CategoryDto dto, int level = 0)
    {
        return new CategoryViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            IsActive = dto.IsActive,
            ParentCategoryId = dto.ParentCategoryId,
            ParentCategoryName = dto.ParentCategoryName,
            Level = level,
            HierarchyPath = BuildHierarchyPath(dto)
        };
    }

    /// <summary>
    /// Convierte lista de CategoryDto a lista de CategoryViewModel con jerarquía
    /// </summary>
    public static List<CategoryViewModel> ToViewModelList(IEnumerable<CategoryDto> dtos)
    {
        var viewModels = new List<CategoryViewModel>();
        var dtoList = dtos.ToList();

        // Primero crear todos los ViewModels
        var viewModelDict = dtoList.ToDictionary(
            dto => dto.Id, 
            dto => ToViewModel(dto)
        );

        // Luego establecer las relaciones jerárquicas
        foreach (var dto in dtoList)
        {
            var viewModel = viewModelDict[dto.Id];
            
            if (dto.ParentCategoryId.HasValue && viewModelDict.ContainsKey(dto.ParentCategoryId.Value))
            {
                var parent = viewModelDict[dto.ParentCategoryId.Value];
                parent.AddSubCategory(viewModel);
            }
            else
            {
                // Es una categoría raíz
                viewModels.Add(viewModel);
            }
        }

        return viewModels.OrderBy(vm => vm.Name).ToList();
    }

    /// <summary>
    /// Convierte CategoryViewModel a CreateCategoryDto para API
    /// </summary>
    public static CreateCategoryDto ToCreateDto(CreateCategoryViewModel viewModel)
    {
        return new CreateCategoryDto
        {
            Name = viewModel.Name.Trim(),
            Description = viewModel.Description?.Trim() ?? string.Empty,
            ParentCategoryId = viewModel.ParentCategoryId
        };
    }

    /// <summary>
    /// Convierte CategoryViewModel a UpdateCategoryDto para API
    /// </summary>
    public static UpdateCategoryDto ToUpdateDto(UpdateCategoryViewModel viewModel)
    {
        return new UpdateCategoryDto
        {
            Name = viewModel.Name.Trim(),
            Description = viewModel.Description?.Trim() ?? string.Empty,
            ParentCategoryId = viewModel.ParentCategoryId
        };
    }

    /// <summary>
    /// Convierte CategoryDto a CreateCategoryViewModel para edición
    /// </summary>
    public static CreateCategoryViewModel ToCreateViewModel(CategoryDto? dto = null)
    {
        if (dto == null)
        {
            return new CreateCategoryViewModel();
        }

        return new CreateCategoryViewModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            ParentCategoryName = dto.ParentCategoryName
        };
    }

    /// <summary>
    /// Convierte CategoryDto a UpdateCategoryViewModel para edición
    /// </summary>
    public static UpdateCategoryViewModel ToUpdateViewModel(CategoryDto dto)
    {
        return new UpdateCategoryViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            ParentCategoryName = dto.ParentCategoryName,
            HasChanges = false
        };
    }

    /// <summary>
    /// Copia datos de CategoryViewModel a UpdateCategoryViewModel
    /// </summary>
    public static void CopyToUpdateViewModel(CategoryViewModel source, UpdateCategoryViewModel target)
    {
        target.Id = source.Id;
        target.Name = source.Name;
        target.Description = source.Description;
        target.ParentCategoryId = source.ParentCategoryId;
        target.ParentCategoryName = source.ParentCategoryName;
        target.MarkAsChanged();
    }

    /// <summary>
    /// Construye la ruta jerárquica de una categoría
    /// </summary>
    private static string BuildHierarchyPath(CategoryDto dto)
    {
        if (string.IsNullOrEmpty(dto.ParentCategoryName))
            return dto.Name;
        
        return $"{dto.ParentCategoryName} > {dto.Name}";
    }

    /// <summary>
    /// Aplana la jerarquía de categorías para mostrar en listas
    /// </summary>
    public static List<CategoryViewModel> FlattenHierarchy(List<CategoryViewModel> categories)
    {
        var flattened = new List<CategoryViewModel>();
        
        foreach (var category in categories.OrderBy(c => c.Name))
        {
            flattened.Add(category);
            FlattenSubCategories(category.SubCategories, flattened, category.Level + 1);
        }
        
        return flattened;
    }

    private static void FlattenSubCategories(List<CategoryViewModel> subCategories, List<CategoryViewModel> flattened, int level)
    {
        foreach (var subCategory in subCategories.OrderBy(c => c.Name))
        {
            subCategory.Level = level;
            flattened.Add(subCategory);
            FlattenSubCategories(subCategory.SubCategories, flattened, level + 1);
        }
    }

    /// <summary>
    /// Valida que los datos del ViewModel sean consistentes antes del mapeo
    /// </summary>
    public static bool ValidateForMapping(CreateCategoryViewModel viewModel, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(viewModel.Name))
            errors.Add("Nombre es requerido");

        return !errors.Any();
    }

    /// <summary>
    /// Valida que los datos del ViewModel sean consistentes antes del mapeo
    /// </summary>
    public static bool ValidateForMapping(UpdateCategoryViewModel viewModel, out List<string> errors)
    {
        errors = new List<string>();

        if (viewModel.Id <= 0)
            errors.Add("ID de categoría inválido");

        if (string.IsNullOrWhiteSpace(viewModel.Name))
            errors.Add("Nombre es requerido");

        if (viewModel.WouldCreateCircularReference(viewModel.ParentCategoryId))
            errors.Add("La categoría padre seleccionada crearía una referencia circular");

        return !errors.Any();
    }

    /// <summary>
    /// Obtiene las categorías disponibles como padre para una categoría específica
    /// </summary>
    public static List<CategoryViewModel> GetAvailableParentCategories(
        List<CategoryViewModel> allCategories, 
        int? excludeCategoryId = null)
    {
        var available = new List<CategoryViewModel>();
        
        foreach (var category in allCategories)
        {
            // Excluir la categoría actual y sus descendientes
            if (excludeCategoryId.HasValue && 
                (category.Id == excludeCategoryId.Value || category.IsDescendantOf(excludeCategoryId.Value)))
            {
                continue;
            }
            
            available.Add(category);
        }
        
        return FlattenHierarchy(available);
    }
}
