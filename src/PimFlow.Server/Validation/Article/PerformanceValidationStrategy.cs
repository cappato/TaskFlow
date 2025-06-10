using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Example of extensible validation strategy for performance concerns
/// Demonstrates Open/Closed Principle - NEW VALIDATOR ADDED WITHOUT MODIFYING EXISTING CODE
/// </summary>
public class PerformanceValidationStrategy : IArticleCreateValidationStrategy
{
    public int Priority => 5; // Lower priority - performance checks last
    public string Name => "Performance Validation";
    public ValidationCategory Category => ValidationCategory.Performance;

    public Task<ValidationResult> ValidateAsync(CreateArticleDto item)
    {
        var warnings = new List<string>();

        // Performance-related validations
        if (item.CustomAttributes?.Count > 50)
            warnings.Add("Artículo tiene demasiados atributos personalizados (>50), esto puede afectar el rendimiento");

        if (item.Description?.Length > 5000)
            warnings.Add("Descripción muy larga (>5000 caracteres), considere resumir");

        // Check for potential performance issues
        if (item.CustomAttributes?.Any(attr => attr.Value?.ToString()?.Length > 1000) == true)
            warnings.Add("Algunos atributos personalizados tienen valores muy largos, esto puede afectar el rendimiento");

        // Example: Check for complex data structures in custom attributes
        if (item.CustomAttributes?.Any(attr => IsComplexValue(attr.Value)) == true)
            warnings.Add("Algunos atributos contienen estructuras de datos complejas, considere simplificar");

        var result = new ValidationResult
        {
            IsValid = true, // Performance issues are warnings, not errors
            Warnings = warnings
        };

        return Task.FromResult(result);
    }

    private static bool IsComplexValue(object? value)
    {
        if (value == null) return false;
        
        var valueString = value.ToString();
        
        // Simple heuristics for complex data
        return valueString?.Contains('[') == true || 
               valueString?.Contains('{') == true ||
               valueString?.Split(',').Length > 10;
    }
}


