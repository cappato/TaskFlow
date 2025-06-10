using PimFlow.Server.Validation;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Security validation strategy - demonstrates OCP by adding new validation
/// without modifying existing validation strategies or pipeline
/// </summary>
public class SecurityValidationStrategy : IArticleCreateValidationStrategy
{
    public string Name => "SecurityValidation";
    public int Priority => 50; // High priority for security
    public ValidationCategory Category => ValidationCategory.Security;

    public async Task<ValidationResult> ValidateAsync(CreateArticleDto item)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Security validations - new functionality without modifying existing code
        await ValidateSuspiciousContent(item, errors);
        await ValidateInputSanitization(item, errors);
        await ValidateBusinessRules(item, errors, warnings);

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };
    }

    private async Task ValidateSuspiciousContent(CreateArticleDto item, List<string> errors)
    {
        // Check for suspicious patterns in text fields
        var suspiciousPatterns = new[] { "<script", "javascript:", "eval(", "onclick=" };

        var fieldsToCheck = new[]
        {
            item.Name,
            item.Description,
            item.Brand
        };

        foreach (var field in fieldsToCheck.Where(f => !string.IsNullOrEmpty(f)))
        {
            foreach (var pattern in suspiciousPatterns)
            {
                if (field.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"Suspicious content detected: {pattern}");
                }
            }
        }

        await Task.CompletedTask;
    }

    private async Task ValidateInputSanitization(CreateArticleDto item, List<string> errors)
    {
        // Validate input length to prevent buffer overflow attacks
        const int maxFieldLength = 1000;

        if (!string.IsNullOrEmpty(item.Name) && item.Name.Length > maxFieldLength)
        {
            errors.Add($"Name exceeds maximum length of {maxFieldLength} characters");
        }

        if (!string.IsNullOrEmpty(item.Description) && item.Description.Length > maxFieldLength * 5)
        {
            errors.Add($"Description exceeds maximum length of {maxFieldLength * 5} characters");
        }

        if (!string.IsNullOrEmpty(item.Brand) && item.Brand.Length > maxFieldLength)
        {
            errors.Add($"Brand exceeds maximum length of {maxFieldLength} characters");
        }

        await Task.CompletedTask;
    }

    private async Task ValidateBusinessRules(CreateArticleDto item, List<string> errors, List<string> warnings)
    {
        // Security-related business rules
        if (!string.IsNullOrEmpty(item.SKU))
        {
            // SKU should not contain special characters that could be used for injection
            var invalidChars = new[] { ';', '\'', '"', '<', '>', '&' };
            if (item.SKU.Any(c => invalidChars.Contains(c)))
            {
                errors.Add("SKU contains invalid characters that could pose security risks");
            }
        }

        // Validate that required security fields are present
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            errors.Add("Name is required for security audit trail");
        }

        await Task.CompletedTask;
    }
}

/// <summary>
/// Additional validation category for security
/// Demonstrates extension of enum-like behavior without modifying existing code
/// </summary>
public static class SecurityValidationCategory
{
    public const string Security = "Security";
    public const string Compliance = "Compliance";
    public const string DataProtection = "DataProtection";
}

/// <summary>
/// Extension methods for ValidationResult to add security-specific functionality
/// Demonstrates OCP by extending existing functionality without modification
/// </summary>
public static class SecurityValidationExtensions
{
    public static ValidationResult AddSecurityWarning(this ValidationResult result, string message)
    {
        result.Warnings.Add($"[SECURITY] {message}");
        return result;
    }

    public static ValidationResult AddSecurityError(this ValidationResult result, string message)
    {
        result.Errors.Add($"[SECURITY] {message}");
        result.IsValid = false;
        return result;
    }

    public static bool HasSecurityIssues(this ValidationResult result)
    {
        return result.Errors.Any(m => m.Contains("[SECURITY]")) ||
               result.Warnings.Any(m => m.Contains("[SECURITY]"));
    }

    public static IEnumerable<string> GetSecurityMessages(this ValidationResult result)
    {
        return result.Errors.Concat(result.Warnings).Where(m => m.Contains("[SECURITY]"));
    }
}
