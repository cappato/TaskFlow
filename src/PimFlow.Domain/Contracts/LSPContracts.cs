using System.ComponentModel.DataAnnotations;

namespace PimFlow.Domain.Contracts;

/// <summary>
/// Behavioral contracts that ensure Liskov Substitution Principle compliance
/// These contracts define the expected behavior that all implementations must follow
/// </summary>
public static class LSPContracts
{
    /// <summary>
    /// Contract for ISpecification implementations
    /// Ensures all implementations behave consistently and are substitutable
    /// </summary>
    public static class SpecificationContract
    {
        /// <summary>
        /// Precondition: Entity parameter must not be null
        /// Postcondition: Must return a boolean result
        /// Invariant: ErrorMessage must never be null or empty
        /// </summary>
        public static void ValidateImplementation<T>(ISpecification<T> specification, T validEntity)
        {
            // Precondition validation
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            // Invariant validation
            if (string.IsNullOrEmpty(specification.ErrorMessage))
                throw new InvalidOperationException("ErrorMessage must never be null or empty (LSP invariant)");

            // Behavioral contract validation
            if (validEntity != null)
            {
                // Must return a boolean (postcondition)
                var result = specification.IsSatisfiedBy(validEntity);
                // Result should be deterministic for same input
                var result2 = specification.IsSatisfiedBy(validEntity);
                if (result != result2)
                    throw new InvalidOperationException("Specification must be deterministic (LSP behavioral contract)");
            }

            // Null handling contract (all implementations must handle null consistently)
            try
            {
                specification.IsSatisfiedBy(default(T)!);
                // If no exception, null handling is implementation-specific but must be consistent
            }
            catch (ArgumentNullException)
            {
                // Expected behavior for null input - this is acceptable
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected exception type for null input: {ex.GetType().Name}. " +
                    "LSP requires consistent exception handling across implementations.");
            }
        }
    }

    /// <summary>
    /// Contract for validation strategy implementations
    /// Ensures all validation strategies are substitutable
    /// </summary>
    public static class ValidationStrategyContract
    {
        /// <summary>
        /// Precondition: Entity and cancellation token must be valid
        /// Postcondition: Must return a ValidationResult with consistent structure
        /// Invariant: Name and Category properties must be consistent
        /// </summary>
        public static async Task ValidateImplementationAsync<T>(
            IValidationStrategy<T> strategy, 
            T validEntity, 
            CancellationToken cancellationToken = default)
        {
            // Precondition validation
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            // Invariant validation
            if (string.IsNullOrEmpty(strategy.Name))
                throw new InvalidOperationException("Strategy Name must never be null or empty (LSP invariant)");

            if (strategy.Priority < 0)
                throw new InvalidOperationException("Strategy Priority must be non-negative (LSP invariant)");

            // Behavioral contract validation
            if (validEntity != null)
            {
                var result = await strategy.ValidateAsync(validEntity, cancellationToken);
                
                // Postcondition: Must return a valid ValidationResult
                if (result == null)
                    throw new InvalidOperationException("ValidationResult must never be null (LSP postcondition)");

                // Behavioral consistency: Source should match strategy name
                if (result.Source != strategy.Name)
                    throw new InvalidOperationException("ValidationResult.Source must match strategy.Name (LSP behavioral contract)");

                // Deterministic behavior for same input
                var result2 = await strategy.ValidateAsync(validEntity, cancellationToken);
                if (result.IsValid != result2.IsValid)
                    throw new InvalidOperationException("Validation must be deterministic for same input (LSP behavioral contract)");
            }

            // Null handling contract
            try
            {
                await strategy.ValidateAsync(default(T)!, cancellationToken);
            }
            catch (ArgumentNullException)
            {
                // Expected behavior - acceptable
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected behavior for cancellation - acceptable
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected exception type for null input: {ex.GetType().Name}. " +
                    "LSP requires consistent exception handling across implementations.");
            }
        }
    }

    /// <summary>
    /// Contract for service implementations
    /// Ensures all service implementations are substitutable through their interfaces
    /// </summary>
    public static class ServiceContract
    {
        /// <summary>
        /// Validates that service implementations maintain behavioral consistency
        /// Required for LSP compliance in service layer
        /// </summary>
        public static async Task ValidateServiceBehaviorAsync<TService, TEntity, TDto>(
            TService service,
            Func<TService, Task<TDto?>> getOperation,
            Func<TService, Task<IEnumerable<TDto>>> getAllOperation)
            where TService : class
            where TDto : class
        {
            // Precondition validation
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            // Behavioral contract: GetAll should never return null
            var allItems = await getAllOperation(service);
            if (allItems == null)
                throw new InvalidOperationException("GetAll operations must never return null (LSP behavioral contract)");

            // Behavioral contract: Get operations should be consistent
            var singleItem = await getOperation(service);
            // If single item exists, it should be included in GetAll results
            if (singleItem != null)
            {
                var allItemsList = allItems.ToList();
                // This is a simplified check - in real scenarios, you'd compare by ID or other unique identifier
                if (!allItemsList.Any())
                    throw new InvalidOperationException("Inconsistent behavior between Get and GetAll operations (LSP violation)");
            }
        }

        /// <summary>
        /// Validates that CRUD operations maintain referential integrity
        /// Essential for LSP compliance in data access layer
        /// </summary>
        public static async Task ValidateCRUDConsistencyAsync<TService, TCreateDto, TUpdateDto, TDto>(
            TService service,
            TCreateDto createDto,
            TUpdateDto updateDto,
            Func<TService, TCreateDto, Task<TDto>> createOperation,
            Func<TService, int, TUpdateDto, Task<TDto?>> updateOperation,
            Func<TService, int, Task<TDto?>> getOperation,
            Func<TService, int, Task<bool>> deleteOperation)
            where TService : class
            where TDto : class
        {
            // Precondition validation
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            // Create operation behavioral contract
            var created = await createOperation(service, createDto);
            if (created == null)
                throw new InvalidOperationException("Create operation must return created entity (LSP postcondition)");

            // Assuming the DTO has an Id property (common pattern)
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("DTO must have Id property for LSP validation");

            var createdId = (int)idProperty.GetValue(created)!;
            if (createdId <= 0)
                throw new InvalidOperationException("Created entity must have valid Id (LSP postcondition)");

            // Get operation behavioral contract
            var retrieved = await getOperation(service, createdId);
            if (retrieved == null)
                throw new InvalidOperationException("Get operation must return existing entity (LSP behavioral contract)");

            // Update operation behavioral contract (if updateDto is provided)
            if (updateDto != null)
            {
                var updated = await updateOperation(service, createdId, updateDto);
                if (updated == null)
                    throw new InvalidOperationException("Update operation must return updated entity (LSP postcondition)");
            }

            // Delete operation behavioral contract
            var deleteResult = await deleteOperation(service, createdId);
            if (!deleteResult)
                throw new InvalidOperationException("Delete operation must return true for existing entity (LSP postcondition)");

            // Verify deletion
            var deletedEntity = await getOperation(service, createdId);
            if (deletedEntity != null)
                throw new InvalidOperationException("Entity must not exist after deletion (LSP postcondition)");
        }
    }
}

/// <summary>
/// Interface for specifications that must follow LSP contracts
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Error message that describes what the specification validates
    /// Invariant: Must never be null or empty
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Determines if the entity satisfies the specification
    /// Precondition: Entity can be null (implementations must handle consistently)
    /// Postcondition: Must return a boolean value
    /// Behavioral contract: Must be deterministic for same input
    /// </summary>
    bool IsSatisfiedBy(T entity);
}

/// <summary>
/// Interface for validation strategies that must follow LSP contracts
/// </summary>
public interface IValidationStrategy<T>
{
    /// <summary>
    /// Name of the validation strategy
    /// Invariant: Must never be null or empty
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Priority for execution order
    /// Invariant: Must be non-negative
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Validates the entity asynchronously
    /// Precondition: Entity can be null (implementations must handle consistently)
    /// Postcondition: Must return a non-null ValidationResult
    /// Behavioral contract: Result.Source must match strategy.Name
    /// </summary>
    Task<ValidationResult> ValidateAsync(T entity, CancellationToken cancellationToken);
}

/// <summary>
/// Validation result that maintains LSP contracts
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public bool HasWarnings { get; set; }
    public string Source { get; set; } = string.Empty;
    public List<string> Messages { get; set; } = new();

    public static ValidationResult Success(string source) => new() { IsValid = true, Source = source };
    public static ValidationResult Warning(string source, string message) => new() 
    { 
        IsValid = true, 
        HasWarnings = true, 
        Source = source, 
        Messages = new List<string> { message } 
    };

    public void AddError(string message)
    {
        IsValid = false;
        Messages.Add(message);
    }

    public void AddWarning(string message)
    {
        HasWarnings = true;
        Messages.Add(message);
    }

    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var combined = new ValidationResult
        {
            IsValid = results.All(r => r.IsValid),
            HasWarnings = results.Any(r => r.HasWarnings),
            Source = "Combined",
            Messages = results.SelectMany(r => r.Messages).ToList()
        };
        return combined;
    }
}
