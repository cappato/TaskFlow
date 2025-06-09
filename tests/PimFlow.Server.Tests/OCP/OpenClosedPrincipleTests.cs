using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using PimFlow.Domain.Enums;
using Xunit;

namespace PimFlow.Server.Tests.OCP;

/// <summary>
/// Tests to validate Open/Closed Principle (OCP) implementation
/// Ensures the system is open for extension but closed for modification
/// </summary>
public class OpenClosedPrincipleTests
{
    [Fact]
    public void ValidationPipeline_ShouldBeExtensibleWithoutModification()
    {
        // Arrange - Create a new validation strategy without modifying existing code
        var customStrategy = new CustomTestValidationStrategy();
        var existingStrategies = new List<IArticleValidationStrategy>
        {
            new BasicFieldValidationStrategy(),
            new BusinessRulesValidationStrategy(null!, null!),
            customStrategy // New strategy added without modifying existing code
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

        // Act - Execute validation pipeline with extended functionality
        var results = new List<ValidationResult>();
        foreach (var strategy in existingStrategies)
        {
            var result = strategy.ValidateAsync(testArticle, CancellationToken.None).Result;
            results.Add(result);
        }

        // Assert - New functionality works without modifying existing code
        results.Should().HaveCount(3, "All strategies should execute");
        results.Should().Contain(r => r.Source == "CustomTestValidation", 
            "Custom strategy should be executed");
        
        // Existing strategies should still work unchanged
        results.Should().Contain(r => r.Source == "BasicFieldValidation", 
            "Existing basic validation should still work");
        results.Should().Contain(r => r.Source == "BusinessRulesValidation", 
            "Existing business rules validation should still work");
    }

    [Fact]
    public void SpecificationPattern_ShouldAllowExtensionWithoutModification()
    {
        // Arrange - Test that new specifications can be added without modifying existing ones
        var baseSpec = new TestSpecification("Base", true);
        var extensionSpec = new TestSpecification("Extension", false);

        // Act - Combine specifications using logical operations (extension)
        var andSpec = baseSpec.And(extensionSpec);
        var orSpec = baseSpec.Or(extensionSpec);
        var notSpec = baseSpec.Not();

        var testEntity = new { Name = "Test" };

        // Assert - Extended functionality works without modifying base specifications
        andSpec.IsSatisfiedBy(testEntity).Should().BeFalse("AND with false should be false");
        orSpec.IsSatisfiedBy(testEntity).Should().BeTrue("OR with true should be true");
        notSpec.IsSatisfiedBy(testEntity).Should().BeFalse("NOT true should be false");

        // Original specification unchanged
        baseSpec.IsSatisfiedBy(testEntity).Should().BeTrue("Original specification should be unchanged");
    }

    [Fact]
    public void ServiceLayer_ShouldSupportExtensionThroughDependencyInjection()
    {
        // Arrange - Test that new services can be added without modifying existing registration
        var services = new ServiceCollection();
        
        // Existing registrations (closed for modification)
        services.AddScoped<IValidationStrategy<CreateArticleDto>, BasicFieldValidationStrategy>();
        
        // New registration (open for extension)
        services.AddScoped<IValidationStrategy<CreateArticleDto>, CustomTestValidationStrategy>();
        
        var serviceProvider = services.BuildServiceProvider();

        // Act - Resolve all validation strategies
        var strategies = serviceProvider.GetServices<IValidationStrategy<CreateArticleDto>>();

        // Assert - Both existing and new strategies are available
        strategies.Should().HaveCount(2, "Both existing and new strategies should be registered");
        strategies.Should().Contain(s => s.GetType() == typeof(BasicFieldValidationStrategy), 
            "Existing strategy should be available");
        strategies.Should().Contain(s => s.GetType() == typeof(CustomTestValidationStrategy), 
            "New strategy should be available");
    }

    [Fact]
    public void ValidationResult_ShouldSupportExtensionWithoutBreakingExistingCode()
    {
        // Arrange - Test that ValidationResult can be extended without breaking existing usage
        var basicResult = ValidationResult.Success("BasicValidation");
        var extendedResult = ValidationResult.Warning("ExtendedValidation", "Custom warning message");

        // Act - Combine results (extension behavior)
        var combinedResult = ValidationResult.Combine(basicResult, extendedResult);

        // Assert - Extended functionality works with existing API
        combinedResult.IsValid.Should().BeTrue("Combined result should be valid when no errors");
        combinedResult.HasWarnings.Should().BeTrue("Combined result should have warnings");
        combinedResult.Messages.Should().Contain("Custom warning message", "Extended message should be preserved");
        
        // Existing behavior unchanged
        basicResult.IsValid.Should().BeTrue("Original result should be unchanged");
        basicResult.HasWarnings.Should().BeFalse("Original result should have no warnings");
    }
}

// Custom test implementations to demonstrate OCP
internal class CustomTestValidationStrategy : IArticleValidationStrategy
{
    public string Name => "CustomTestValidation";
    public int Priority => 100;
    public ValidationCategory Category => ValidationCategory.Business;

    public Task<ValidationResult> ValidateAsync(CreateArticleDto entity, CancellationToken cancellationToken)
    {
        // Custom validation logic - extension without modification
        var result = ValidationResult.Success("CustomTestValidation");
        return Task.FromResult(result);
    }
}

internal class TestSpecification : ISpecification<object>
{
    private readonly bool _result;
    
    public TestSpecification(string name, bool result)
    {
        ErrorMessage = $"Test specification: {name}";
        _result = result;
    }

    public string ErrorMessage { get; }

    public bool IsSatisfiedBy(object entity)
    {
        return _result;
    }
}

// Interface for demonstration
internal interface ISpecification<T>
{
    string ErrorMessage { get; }
    bool IsSatisfiedBy(T entity);
}

// Extension methods for specifications (OCP in action)
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
        _left = left;
        _right = right;
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
        _left = left;
        _right = right;
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
        _specification = specification;
    }

    public string ErrorMessage => $"NOT ({_specification.ErrorMessage})";

    public bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
}
