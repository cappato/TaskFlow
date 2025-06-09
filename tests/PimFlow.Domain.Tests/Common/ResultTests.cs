using FluentAssertions;
using PimFlow.Domain.Common;
using Xunit;

namespace PimFlow.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithValue_ShouldCreateFailedResultWithDefaultValue()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure<string>(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Constructor_SuccessWithError_ShouldThrowException()
    {
        // Act & Assert
        var action = () => new TestableResult(true, "error message");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Un resultado exitoso no puede tener error");
    }

    [Fact]
    public void Constructor_FailureWithoutError_ShouldThrowException()
    {
        // Act & Assert
        var action = () => new TestableResult(false, string.Empty);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Un resultado fallido debe tener un mensaje de error");
    }

    [Fact]
    public void Map_SuccessfulResult_ShouldTransformValue()
    {
        // Arrange
        var result = Result.Success(5);

        // Act
        var mappedResult = result.Map(x => x.ToString());

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be("5");
    }

    [Fact]
    public void Map_FailedResult_ShouldReturnFailedResult()
    {
        // Arrange
        var result = Result.Failure<int>("error");

        // Act
        var mappedResult = result.Map(x => x.ToString());

        // Assert
        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Error.Should().Be("error");
    }

    [Fact]
    public void Map_WithException_ShouldReturnFailedResult()
    {
        // Arrange
        var result = Result.Success(5);

        // Act
        var mappedResult = result.Map<string>(x => throw new InvalidOperationException("mapping error"));

        // Assert
        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Error.Should().Be("mapping error");
    }

    [Fact]
    public void OnSuccess_SuccessfulResult_ShouldExecuteAction()
    {
        // Arrange
        var result = Result.Success("test");
        var actionExecuted = false;

        // Act
        var returnedResult = result.OnSuccess(value =>
        {
            actionExecuted = true;
            value.Should().Be("test");
        });

        // Assert
        actionExecuted.Should().BeTrue();
        returnedResult.Should().BeSameAs(result);
    }

    [Fact]
    public void OnSuccess_FailedResult_ShouldNotExecuteAction()
    {
        // Arrange
        var result = Result.Failure<string>("error");
        var actionExecuted = false;

        // Act
        var returnedResult = result.OnSuccess(value => actionExecuted = true);

        // Assert
        actionExecuted.Should().BeFalse();
        returnedResult.Should().BeSameAs(result);
    }

    [Fact]
    public void OnFailure_FailedResult_ShouldExecuteAction()
    {
        // Arrange
        var result = Result.Failure<string>("test error");
        var actionExecuted = false;

        // Act
        var returnedResult = result.OnFailure(error =>
        {
            actionExecuted = true;
            error.Should().Be("test error");
        });

        // Assert
        actionExecuted.Should().BeTrue();
        returnedResult.Should().BeSameAs(result);
    }

    [Fact]
    public void OnFailure_SuccessfulResult_ShouldNotExecuteAction()
    {
        // Arrange
        var result = Result.Success("test");
        var actionExecuted = false;

        // Act
        var returnedResult = result.OnFailure(error => actionExecuted = true);

        // Assert
        actionExecuted.Should().BeFalse();
        returnedResult.Should().BeSameAs(result);
    }

    [Fact]
    public void Bind_SuccessfulResult_ShouldExecuteFunction()
    {
        // Arrange
        var result = Result.Success(5);

        // Act
        var boundResult = result.Bind(x => Result.Success(x * 2));

        // Assert
        boundResult.IsSuccess.Should().BeTrue();
        boundResult.Value.Should().Be(10);
    }

    [Fact]
    public void Bind_FailedResult_ShouldReturnFailedResult()
    {
        // Arrange
        var result = Result.Failure<int>("error");

        // Act
        var boundResult = result.Bind(x => Result.Success(x * 2));

        // Assert
        boundResult.IsSuccess.Should().BeFalse();
        boundResult.Error.Should().Be("error");
    }

    [Fact]
    public void GetValueOrThrow_SuccessfulResult_ShouldReturnValue()
    {
        // Arrange
        var result = Result.Success("test value");

        // Act
        var value = result.GetValueOrThrow();

        // Assert
        value.Should().Be("test value");
    }

    [Fact]
    public void GetValueOrThrow_FailedResult_ShouldThrowException()
    {
        // Arrange
        var result = Result.Failure<string>("error message");

        // Act & Assert
        var action = () => result.GetValueOrThrow();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("error message");
    }

    [Fact]
    public void GetValueOrDefault_SuccessfulResult_ShouldReturnValue()
    {
        // Arrange
        var result = Result.Success("test value");

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        value.Should().Be("test value");
    }

    [Fact]
    public void GetValueOrDefault_FailedResult_ShouldReturnDefault()
    {
        // Arrange
        var result = Result.Failure<string>("error");

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        value.Should().Be("default");
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessfulResult()
    {
        // Act
        Result<string> result = "test value";

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test value");
    }

    [Fact]
    public void Combine_AllSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        // Act
        var combined = ResultExtensions.Combine(result1, result2, result3);

        // Assert
        combined.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Combine_OneFailure_ShouldReturnFirstFailure()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Failure("first error");
        var result3 = Result.Failure("second error");

        // Act
        var combined = ResultExtensions.Combine(result1, result2, result3);

        // Assert
        combined.IsSuccess.Should().BeFalse();
        combined.Error.Should().Be("first error");
    }

    [Fact]
    public void CombineAll_AllSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        // Act
        var combined = ResultExtensions.CombineAll(result1, result2, result3);

        // Assert
        combined.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CombineAll_MultipleFailures_ShouldReturnAllErrors()
    {
        // Arrange
        var result1 = Result.Success();
        var result2 = Result.Failure("first error");
        var result3 = Result.Failure("second error");

        // Act
        var combined = ResultExtensions.CombineAll(result1, result2, result3);

        // Assert
        combined.IsSuccess.Should().BeFalse();
        combined.Error.Should().Be("first error; second error");
    }

    // Helper class to test protected constructor
    private class TestableResult : Result
    {
        public TestableResult(bool isSuccess, string error) : base(isSuccess, error)
        {
        }
    }
}
