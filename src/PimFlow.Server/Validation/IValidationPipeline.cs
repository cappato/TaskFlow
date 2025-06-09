namespace PimFlow.Server.Validation;

/// <summary>
/// Pipeline for executing multiple validation strategies
/// Follows Open/Closed Principle - new validators can be added without modifying existing code
/// </summary>
/// <typeparam name="T">Type of object to validate</typeparam>
public interface IValidationPipeline<T>
{
    /// <summary>
    /// Executes all registered validation strategies for the given item
    /// </summary>
    /// <param name="item">Object to validate</param>
    /// <returns>Combined validation result</returns>
    Task<ValidationResult> ValidateAsync(T item);

    /// <summary>
    /// Registers a validation strategy in the pipeline
    /// </summary>
    /// <param name="strategy">Validation strategy to register</param>
    void RegisterStrategy(IValidationStrategy<T> strategy);

    /// <summary>
    /// Gets all registered strategies ordered by priority
    /// </summary>
    IEnumerable<IValidationStrategy<T>> GetStrategies();
}

/// <summary>
/// Default implementation of validation pipeline
/// </summary>
/// <typeparam name="T">Type of object to validate</typeparam>
public class ValidationPipeline<T> : IValidationPipeline<T>
{
    private readonly List<IValidationStrategy<T>> _strategies = new();

    public async Task<ValidationResult> ValidateAsync(T item)
    {
        var results = new List<ValidationResult>();
        
        // Execute strategies in priority order
        var orderedStrategies = _strategies.OrderBy(s => s.Priority);
        
        foreach (var strategy in orderedStrategies)
        {
            var result = await strategy.ValidateAsync(item);
            results.Add(result);
            
            // Optional: Stop on first failure (can be configurable)
            // if (!result.IsValid) break;
        }
        
        return ValidationResult.Combine(results.ToArray());
    }

    public void RegisterStrategy(IValidationStrategy<T> strategy)
    {
        if (strategy == null)
            throw new ArgumentNullException(nameof(strategy));
            
        _strategies.Add(strategy);
    }

    public IEnumerable<IValidationStrategy<T>> GetStrategies()
    {
        return _strategies.OrderBy(s => s.Priority).ToList();
    }
}
