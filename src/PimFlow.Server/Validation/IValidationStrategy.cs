namespace PimFlow.Server.Validation;

/// <summary>
/// Base interface for validation strategies
/// Follows Open/Closed Principle - open for extension, closed for modification
/// </summary>
/// <typeparam name="T">Type of object to validate</typeparam>
public interface IValidationStrategy<T>
{
    /// <summary>
    /// Validates the given object
    /// </summary>
    /// <param name="item">Object to validate</param>
    /// <returns>Validation result with success status and errors</returns>
    Task<ValidationResult> ValidateAsync(T item);

    /// <summary>
    /// Priority of this validation strategy (lower number = higher priority)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Name of the validation strategy for logging/debugging
    /// </summary>
    string Name { get; }
}

/// <summary>
/// Validation result with success status and error messages
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };
    
    public static ValidationResult Failure(params string[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors.ToList() 
    };

    public static ValidationResult Warning(params string[] warnings) => new()
    {
        IsValid = true,
        Warnings = warnings.ToList()
    };

    /// <summary>
    /// Combines multiple validation results
    /// </summary>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var combined = new ValidationResult { IsValid = true };
        
        foreach (var result in results)
        {
            if (!result.IsValid)
                combined.IsValid = false;
                
            combined.Errors.AddRange(result.Errors);
            combined.Warnings.AddRange(result.Warnings);
        }
        
        return combined;
    }
}
