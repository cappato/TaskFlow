using PimFlow.Server.Validation;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Security validation strategy - demonstrates OCP by adding new validation
/// without modifying existing validation strategies or pipeline
/// </summary>
public class SecurityValidationStrategy : IArticleValidationStrategy
{
    public string Name => "SecurityValidation";
    public int Priority => 50; // High priority for security
    public ValidationCategory Category => ValidationCategory.Security;

    public async Task<ValidationResult> ValidateAsync(CreateArticleDto entity, CancellationToken cancellationToken)
    {
        var result = ValidationResult.Success(Name);

        // Security validations - new functionality without modifying existing code
        await ValidateSuspiciousContent(entity, result);
        await ValidateInputSanitization(entity, result);
        await ValidateBusinessRules(entity, result);

        return result;
    }

    private async Task ValidateSuspiciousContent(CreateArticleDto entity, ValidationResult result)
    {
        // Check for suspicious patterns in text fields
        var suspiciousPatterns = new[] { "<script", "javascript:", "eval(", "onclick=" };
        
        var fieldsToCheck = new[]
        {
            entity.Name,
            entity.Description,
            entity.Brand
        };

        foreach (var field in fieldsToCheck.Where(f => !string.IsNullOrEmpty(f)))
        {
            foreach (var pattern in suspiciousPatterns)
            {
                if (field.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    result.AddError($"Suspicious content detected: {pattern}");
                }
            }
        }

        await Task.CompletedTask;
    }

    private async Task ValidateInputSanitization(CreateArticleDto entity, ValidationResult result)
    {
        // Validate input length to prevent buffer overflow attacks
        const int maxFieldLength = 1000;

        if (!string.IsNullOrEmpty(entity.Name) && entity.Name.Length > maxFieldLength)
        {
            result.AddError($"Name exceeds maximum length of {maxFieldLength} characters");
        }

        if (!string.IsNullOrEmpty(entity.Description) && entity.Description.Length > maxFieldLength * 5)
        {
            result.AddError($"Description exceeds maximum length of {maxFieldLength * 5} characters");
        }

        if (!string.IsNullOrEmpty(entity.Brand) && entity.Brand.Length > maxFieldLength)
        {
            result.AddError($"Brand exceeds maximum length of {maxFieldLength} characters");
        }

        await Task.CompletedTask;
    }

    private async Task ValidateBusinessRules(CreateArticleDto entity, ValidationResult result)
    {
        // Security-related business rules
        if (!string.IsNullOrEmpty(entity.SKU))
        {
            // SKU should not contain special characters that could be used for injection
            var invalidChars = new[] { ';', '\'', '"', '<', '>', '&' };
            if (entity.SKU.Any(c => invalidChars.Contains(c)))
            {
                result.AddError("SKU contains invalid characters that could pose security risks");
            }
        }

        // Validate that required security fields are present
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("Name is required for security audit trail");
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
        result.AddWarning($"[SECURITY] {message}");
        return result;
    }

    public static ValidationResult AddSecurityError(this ValidationResult result, string message)
    {
        result.AddError($"[SECURITY] {message}");
        return result;
    }

    public static bool HasSecurityIssues(this ValidationResult result)
    {
        return result.Messages.Any(m => m.Contains("[SECURITY]"));
    }

    public static IEnumerable<string> GetSecurityMessages(this ValidationResult result)
    {
        return result.Messages.Where(m => m.Contains("[SECURITY]"));
    }
}
