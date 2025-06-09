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

/// <summary>
/// Example of security validation strategy
/// Demonstrates how easy it is to add new validation concerns
/// </summary>
public class SecurityValidationStrategy : IArticleCreateValidationStrategy
{
    public int Priority => 4; // High priority for security
    public string Name => "Security Validation";
    public ValidationCategory Category => ValidationCategory.Security;

    public Task<ValidationResult> ValidateAsync(CreateArticleDto item)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Security validations
        if (ContainsSuspiciousContent(item.Name))
            errors.Add("Nombre contiene contenido potencialmente malicioso");

        if (ContainsSuspiciousContent(item.Description))
            warnings.Add("Descripción contiene contenido que requiere revisión");

        if (item.CustomAttributes?.Any(attr => ContainsSuspiciousContent(attr.Value?.ToString())) == true)
            warnings.Add("Algunos atributos personalizados contienen contenido que requiere revisión");

        // Check for injection attempts
        if (ContainsScriptTags(item.Description))
            errors.Add("Descripción contiene etiquetas de script no permitidas");

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };

        return Task.FromResult(result);
    }

    private static bool ContainsSuspiciousContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return false;
        
        var suspiciousPatterns = new[] { "<script", "javascript:", "vbscript:", "onload=", "onerror=" };
        return suspiciousPatterns.Any(pattern => 
            content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ContainsScriptTags(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return false;
        
        return content.Contains("<script", StringComparison.OrdinalIgnoreCase) ||
               content.Contains("</script>", StringComparison.OrdinalIgnoreCase);
    }
}
