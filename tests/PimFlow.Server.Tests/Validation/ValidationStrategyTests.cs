using FluentAssertions;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Validation;

/// <summary>
/// Tests demonstrating Open/Closed Principle through Strategy Pattern
/// Shows how new validation strategies can be added without modifying existing code
/// </summary>
public class ValidationStrategyTests
{
    [Fact]
    public async Task BasicFieldValidationStrategy_WithValidData_ShouldPass()
    {
        // Arrange
        var strategy = new BasicFieldValidationStrategy();
        var validDto = new CreateArticleDto
        {
            SKU = "VALIDSKU001",
            Name = "Valid Article Name",
            Description = "Valid description",
            Brand = "Valid Brand"
        };

        // Act
        var result = await strategy.ValidateAsync(validDto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task BasicFieldValidationStrategy_WithInvalidData_ShouldFail()
    {
        // Arrange
        var strategy = new BasicFieldValidationStrategy();
        var invalidDto = new CreateArticleDto
        {
            SKU = "", // Invalid - empty
            Name = "", // Invalid - empty
            Description = new string('x', 1001), // Invalid - too long
            Brand = new string('y', 101) // Invalid - too long
        };

        // Act
        var result = await strategy.ValidateAsync(invalidDto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("SKU es requerido");
        result.Errors.Should().Contain("Nombre es requerido");
        result.Errors.Should().Contain("Descripción debe tener entre 0 y 1000 caracteres");
        result.Errors.Should().Contain("Marca debe tener entre 2 y 100 caracteres");
    }

    [Fact]
    public async Task PerformanceValidationStrategy_WithTooManyAttributes_ShouldWarn()
    {
        // Arrange
        var strategy = new PerformanceValidationStrategy();
        var dto = new CreateArticleDto
        {
            SKU = "PERFTEST001",
            Name = "Performance Test Article",
            CustomAttributes = Enumerable.Range(1, 60)
                .ToDictionary(i => $"attr{i}", i => (object)$"value{i}")
        };

        // Act
        var result = await strategy.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue(); // Performance issues are warnings, not errors
        result.Warnings.Should().Contain(w => w.Contains("demasiados atributos personalizados"));
    }

    [Fact]
    public async Task SecurityValidationStrategy_WithSuspiciousContent_ShouldFail()
    {
        // Arrange
        var strategy = new SecurityValidationStrategy();
        var suspiciousDto = new CreateArticleDto
        {
            SKU = "SECTEST001",
            Name = "<script>alert('xss')</script>", // Malicious content
            Description = "Normal description with javascript: injection"
        };

        // Act
        var result = await strategy.ValidateAsync(suspiciousDto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Suspicious content detected"));
        result.Errors.Should().HaveCountGreaterThan(0, "Should detect suspicious content");
    }

    [Fact]
    public async Task ValidationPipeline_WithMultipleStrategies_ShouldCombineResults()
    {
        // Arrange
        var pipeline = new ValidationPipeline<CreateArticleDto>();
        pipeline.RegisterStrategy(new BasicFieldValidationStrategy());
        pipeline.RegisterStrategy(new PerformanceValidationStrategy());
        pipeline.RegisterStrategy(new SecurityValidationStrategy());

        var dto = new CreateArticleDto
        {
            SKU = "", // Basic validation error
            Name = "<script>alert('xss')</script>", // Security error
            Description = new string('x', 6000), // Performance warning
            CustomAttributes = Enumerable.Range(1, 60)
                .ToDictionary(i => $"attr{i}", i => (object)"value") // Performance warning
        };

        // Act
        var result = await pipeline.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse(); // Should fail due to basic validation
        result.Errors.Should().Contain("SKU es requerido"); // From basic validation
        result.Warnings.Should().Contain(w => w.Contains("demasiados atributos")); // From performance validation
        result.Errors.Should().Contain(e => e.Contains("Suspicious content detected")); // From security validation
    }

    [Fact]
    public void ValidationPipeline_StrategiesExecuteInPriorityOrder()
    {
        // Arrange
        var pipeline = new ValidationPipeline<CreateArticleDto>();
        
        // Register in reverse priority order to test sorting
        pipeline.RegisterStrategy(new PerformanceValidationStrategy()); // Priority 5
        pipeline.RegisterStrategy(new SecurityValidationStrategy()); // Priority 4
        pipeline.RegisterStrategy(new BasicFieldValidationStrategy()); // Priority 1

        // Act
        var strategies = pipeline.GetStrategies().ToList();

        // Assert
        strategies[0].Should().BeOfType<BasicFieldValidationStrategy>(); // Priority 1
        strategies[1].Should().BeOfType<PerformanceValidationStrategy>(); // Priority 5
        strategies[2].Should().BeOfType<SecurityValidationStrategy>(); // Priority 50
    }

    [Fact]
    public void ValidationResult_Combine_ShouldMergeMultipleResults()
    {
        // Arrange
        var result1 = ValidationResult.Failure("Error 1");
        var result2 = ValidationResult.Warning("Warning 1");
        var result3 = new ValidationResult
        {
            IsValid = false,
            Errors = new List<string> { "Error 2" },
            Warnings = new List<string> { "Warning 2" }
        };

        // Act
        var combined = ValidationResult.Combine(result1, result2, result3);

        // Assert
        combined.IsValid.Should().BeFalse(); // Should be false if any result is invalid
        combined.Errors.Should().Contain("Error 1");
        combined.Errors.Should().Contain("Error 2");
        combined.Warnings.Should().Contain("Warning 1");
        combined.Warnings.Should().Contain("Warning 2");
    }
}
