using FluentAssertions;
using PimFlow.Domain.Contracts;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using PimFlow.Domain.Enums;
using Xunit;

namespace PimFlow.Server.Tests.LSP;

/// <summary>
/// Tests that validate LSP compliance using behavioral contracts
/// Ensures all implementations can be substituted without breaking functionality
/// </summary>
public class LSPComplianceTests
{
    [Fact]
    public async Task ValidationStrategies_ShouldComplyWithLSPContracts()
    {
        // Arrange - Create different validation strategy implementations
        var strategies = new List<IArticleValidationStrategy>
        {
            new BasicFieldValidationStrategy(),
            new PerformanceValidationStrategy(),
            // Note: BusinessRulesValidationStrategy requires dependencies, so we'll test it separately
        };

        var testArticle = new CreateArticleDto
        {
            SKU = "TEST-001",
            Name = "Test Article",
            Description = "Test Description",
            Type = ArticleType.SimpleProduct,
            Brand = "Test Brand",
            CategoryId = 1
        };

        // Act & Assert - Each strategy must comply with LSP contracts
        foreach (var strategy in strategies)
        {
            // Validate using LSP behavioral contracts
            await LSPContracts.ValidationStrategyContract.ValidateImplementationAsync(
                strategy, testArticle, CancellationToken.None);

            // Additional LSP validation: All strategies should handle same input consistently
            var result1 = await strategy.ValidateAsync(testArticle, CancellationToken.None);
            var result2 = await strategy.ValidateAsync(testArticle, CancellationToken.None);

            result1.IsValid.Should().Be(result2.IsValid, 
                $"Strategy {strategy.Name} must be deterministic (LSP requirement)");
            result1.Source.Should().Be(strategy.Name, 
                $"Strategy {strategy.Name} must set correct source (LSP contract)");
        }
    }

    [Fact]
    public void SpecificationPattern_ShouldComplyWithLSPContracts()
    {
        // Arrange - Create test specifications
        var specifications = new List<ISpecification<object>>
        {
            new LSPTestSpecification("Always True", true),
            new LSPTestSpecification("Always False", false),
            new LSPTestSpecification("Complex Logic", true)
        };

        var testEntity = new { Name = "Test Entity" };

        // Act & Assert - Each specification must comply with LSP contracts
        foreach (var specification in specifications)
        {
            // Validate using LSP behavioral contracts
            LSPContracts.SpecificationContract.ValidateImplementation(specification, testEntity);

            // Additional LSP validation: Logical operations should work consistently
            var andSpec = specification.And(new TestSpecification("Helper", true));
            var orSpec = specification.Or(new TestSpecification("Helper", false));
            var notSpec = specification.Not();

            // All derived specifications should also comply with contracts
            LSPContracts.SpecificationContract.ValidateImplementation(andSpec, testEntity);
            LSPContracts.SpecificationContract.ValidateImplementation(orSpec, testEntity);
            LSPContracts.SpecificationContract.ValidateImplementation(notSpec, testEntity);
        }
    }

    [Fact]
    public void ValidationResult_ShouldMaintainLSPInvariants()
    {
        // Arrange - Create different types of validation results
        var successResult = ValidationResult.Success("TestStrategy");
        var warningResult = ValidationResult.Warning("TestStrategy", "Warning message");
        var errorResult = ValidationResult.Success("TestStrategy");
        errorResult.AddError("Error message");

        var results = new[] { successResult, warningResult, errorResult };

        // Act & Assert - All results should maintain LSP invariants
        foreach (var result in results)
        {
            // LSP Invariant: Source should never be null or empty
            result.Source.Should().NotBeNullOrEmpty("Source is an LSP invariant");

            // LSP Invariant: Messages collection should never be null
            result.Messages.Should().NotBeNull("Messages collection is an LSP invariant");

            // LSP Behavioral contract: IsValid should be consistent with error presence
            if (result.Messages.Any(m => m.Contains("Error") || m.Contains("error")))
            {
                result.IsValid.Should().BeFalse("Results with errors should be invalid (LSP behavioral contract)");
            }
        }

        // LSP Behavioral contract: Combine operation should be associative
        var combined1 = ValidationResult.Combine(successResult, warningResult);
        var combined2 = ValidationResult.Combine(warningResult, successResult);
        
        combined1.IsValid.Should().Be(combined2.IsValid, "Combine should be commutative (LSP behavioral contract)");
        combined1.HasWarnings.Should().Be(combined2.HasWarnings, "Combine should be commutative (LSP behavioral contract)");
    }

    [Fact]
    public async Task ValidationPipeline_ShouldMaintainLSPSubstitutability()
    {
        // Arrange - Create validation pipeline with different strategies
        var strategies = new List<IValidationStrategy<CreateArticleDto>>
        {
            new BasicFieldValidationStrategy(),
            new PerformanceValidationStrategy()
        };

        var testArticle = new CreateArticleDto
        {
            SKU = "TEST-001",
            Name = "Test Article",
            Description = "Test Description",
            Type = ArticleType.SimpleProduct,
            Brand = "Test Brand",
            CategoryId = 1
        };

        // Act - Execute pipeline with different strategy combinations
        var results = new List<ValidationResult>();
        foreach (var strategy in strategies)
        {
            var result = await strategy.ValidateAsync(testArticle, CancellationToken.None);
            results.Add(result);
        }

        // Assert - All strategies should be substitutable in the pipeline
        foreach (var result in results)
        {
            // LSP requirement: All strategies should return valid ValidationResult
            result.Should().NotBeNull("All strategies must return non-null results (LSP postcondition)");
            result.Source.Should().NotBeNullOrEmpty("All strategies must set source (LSP contract)");
        }

        // LSP requirement: Pipeline should work with any combination of strategies
        var combinedResult = ValidationResult.Combine(results.ToArray());
        combinedResult.Should().NotBeNull("Combined result should be valid (LSP requirement)");
        combinedResult.IsValid.Should().Be(results.All(r => r.IsValid), 
            "Combined result should reflect all individual results (LSP behavioral contract)");
    }

    [Fact]
    public void PolymorphicSubstitution_ShouldWorkWithoutBreaking()
    {
        // Arrange - Test polymorphic substitution with different implementations
        var specifications = new List<ISpecification<string>>
        {
            new StringLengthSpecification(5),
            new StringContentSpecification("test"),
            new StringNotEmptySpecification()
        };

        var testStrings = new[] { "test", "testing", "", "short" };

        // Act & Assert - All implementations should be substitutable
        foreach (var spec in specifications)
        {
            foreach (var testString in testStrings)
            {
                // LSP requirement: Should not throw unexpected exceptions
                var act = () => spec.IsSatisfiedBy(testString);
                act.Should().NotThrow($"Specification {spec.GetType().Name} should handle all inputs gracefully");

                // LSP requirement: Should return consistent results
                var result1 = spec.IsSatisfiedBy(testString);
                var result2 = spec.IsSatisfiedBy(testString);
                result1.Should().Be(result2, "Results should be deterministic (LSP requirement)");
            }

            // LSP requirement: Should handle null consistently
            var nullHandling = () => spec.IsSatisfiedBy(null!);
            nullHandling.Should().Throw<ArgumentNullException>("All implementations should handle null consistently");
        }
    }

    [Fact]
    public void InterfaceSubstitution_ShouldMaintainBehavioralContracts()
    {
        // Arrange - Test that interfaces can be substituted without breaking contracts
        var validationStrategies = new List<IValidationStrategy<CreateArticleDto>>
        {
            new BasicFieldValidationStrategy(),
            new PerformanceValidationStrategy()
        };

        // Act & Assert - All implementations should maintain the same behavioral contracts
        foreach (var strategy in validationStrategies)
        {
            // LSP Contract: Name should be consistent
            strategy.Name.Should().NotBeNullOrEmpty("Name is part of the interface contract");

            // LSP Contract: Priority should be valid
            strategy.Priority.Should().BeGreaterOrEqualTo(0, "Priority should be non-negative");

            // LSP Contract: Category should be valid enum value
            strategy.Category.Should().BeDefined("Category should be a valid enum value");

            // LSP Behavioral contract: Same strategy should have same properties
            var name1 = strategy.Name;
            var name2 = strategy.Name;
            name1.Should().Be(name2, "Properties should be consistent (LSP invariant)");
        }
    }
}

// Test implementations for LSP validation
internal class LSPTestSpecification : ISpecification<object>
{
    private readonly bool _result;

    public LSPTestSpecification(string name, bool result)
    {
        ErrorMessage = $"Test specification: {name}";
        _result = result;
    }

    public string ErrorMessage { get; }

    public bool IsSatisfiedBy(object entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return _result;
    }
}

internal class StringLengthSpecification : ISpecification<string>
{
    private readonly int _maxLength;

    public StringLengthSpecification(int maxLength)
    {
        _maxLength = maxLength;
        ErrorMessage = $"String must be no longer than {maxLength} characters";
    }

    public string ErrorMessage { get; }

    public bool IsSatisfiedBy(string entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return entity.Length <= _maxLength;
    }
}

internal class StringContentSpecification : ISpecification<string>
{
    private readonly string _requiredContent;

    public StringContentSpecification(string requiredContent)
    {
        _requiredContent = requiredContent;
        ErrorMessage = $"String must contain '{requiredContent}'";
    }

    public string ErrorMessage { get; }

    public bool IsSatisfiedBy(string entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return entity.Contains(_requiredContent);
    }
}

internal class StringNotEmptySpecification : ISpecification<string>
{
    public string ErrorMessage => "String must not be empty";

    public bool IsSatisfiedBy(string entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return !string.IsNullOrEmpty(entity);
    }
}

// Extension methods for specifications (demonstrating LSP with composition)
internal static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}

internal class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public string ErrorMessage => $"({_left.ErrorMessage}) AND ({_right.ErrorMessage})";

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }
}

internal class OrSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public string ErrorMessage => $"({_left.ErrorMessage}) OR ({_right.ErrorMessage})";

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }
}

internal class NotSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public string ErrorMessage => $"NOT ({_specification.ErrorMessage})";

    public bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
}
