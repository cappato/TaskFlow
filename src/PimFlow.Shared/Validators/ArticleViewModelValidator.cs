using FluentValidation;
using PimFlow.Shared.ViewModels;
using PimFlow.Shared.Enums;

namespace PimFlow.Shared.Validators;

/// <summary>
/// Validador para CreateArticleViewModel
/// Contiene validaciones específicas de UI separadas de las validaciones de negocio
/// </summary>
public class CreateArticleViewModelValidator : AbstractValidator<CreateArticleViewModel>
{
    public CreateArticleViewModelValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU es requerido")
            .Length(3, 50)
            .WithMessage("SKU debe tener entre 3 y 50 caracteres")
            .Matches(@"^[A-Z0-9-]+$")
            .WithMessage("SKU solo puede contener letras mayúsculas, números y guiones");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nombre es requerido")
            .Length(2, 200)
            .WithMessage("Nombre debe tener entre 2 y 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Descripción no puede exceder 1000 caracteres");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo es requerido")
            .Must(BeValidArticleType)
            .WithMessage("Tipo de artículo inválido");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Marca es requerida")
            .Length(2, 100)
            .WithMessage("Marca debe tener entre 2 y 100 caracteres");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("ID de categoría debe ser mayor a 0");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0)
            .When(x => x.SupplierId.HasValue)
            .WithMessage("ID de proveedor debe ser mayor a 0");

        RuleFor(x => x.CustomAttributes)
            .NotNull()
            .WithMessage("Atributos personalizados no pueden ser nulos");
    }

    private bool BeValidArticleType(string type)
    {
        return Enum.TryParse<ArticleType>(type, out _);
    }
}

/// <summary>
/// Validador para UpdateArticleViewModel
/// </summary>
public class UpdateArticleViewModelValidator : AbstractValidator<UpdateArticleViewModel>
{
    public UpdateArticleViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID de artículo debe ser mayor a 0");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU es requerido")
            .Length(3, 50)
            .WithMessage("SKU debe tener entre 3 y 50 caracteres")
            .Matches(@"^[A-Z0-9-]+$")
            .WithMessage("SKU solo puede contener letras mayúsculas, números y guiones");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nombre es requerido")
            .Length(2, 200)
            .WithMessage("Nombre debe tener entre 2 y 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Descripción no puede exceder 1000 caracteres");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo es requerido")
            .Must(BeValidArticleType)
            .WithMessage("Tipo de artículo inválido");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Marca es requerida")
            .Length(2, 100)
            .WithMessage("Marca debe tener entre 2 y 100 caracteres");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("ID de categoría debe ser mayor a 0");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0)
            .When(x => x.SupplierId.HasValue)
            .WithMessage("ID de proveedor debe ser mayor a 0");

        RuleFor(x => x.CustomAttributes)
            .NotNull()
            .WithMessage("Atributos personalizados no pueden ser nulos");
    }

    private bool BeValidArticleType(string type)
    {
        return Enum.TryParse<ArticleType>(type, out _);
    }
}

/// <summary>
/// Extensiones para validadores de artículos
/// </summary>
public static class ArticleValidatorExtensions
{
    /// <summary>
    /// Valida un CreateArticleViewModel y retorna ApiResponse
    /// </summary>
    public static async Task<PimFlow.Shared.Common.ApiResponse> ValidateAsync(
        this CreateArticleViewModelValidator validator, 
        CreateArticleViewModel model)
    {
        var result = await validator.ValidateAsync(model);
        
        if (result.IsValid)
            return PimFlow.Shared.Common.ApiResponse.Success();
        
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        return PimFlow.Shared.Common.ApiResponse.ValidationError(errors);
    }

    /// <summary>
    /// Valida un UpdateArticleViewModel y retorna ApiResponse
    /// </summary>
    public static async Task<PimFlow.Shared.Common.ApiResponse> ValidateAsync(
        this UpdateArticleViewModelValidator validator, 
        UpdateArticleViewModel model)
    {
        var result = await validator.ValidateAsync(model);
        
        if (result.IsValid)
            return PimFlow.Shared.Common.ApiResponse.Success();
        
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        return PimFlow.Shared.Common.ApiResponse.ValidationError(errors);
    }

    /// <summary>
    /// Valida solo campos específicos de un modelo
    /// </summary>
    public static async Task<PimFlow.Shared.Common.ApiResponse> ValidatePropertyAsync<T>(
        this AbstractValidator<T> validator,
        T model,
        params string[] propertyNames)
    {
        var result = await validator.ValidateAsync(model, options =>
        {
            options.IncludeProperties(propertyNames);
        });
        
        if (result.IsValid)
            return PimFlow.Shared.Common.ApiResponse.Success();
        
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        return PimFlow.Shared.Common.ApiResponse.ValidationError(errors);
    }
}
