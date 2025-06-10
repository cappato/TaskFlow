using FluentAssertions;
using PimFlow.Shared.Common;
using PimFlow.Domain.Common;
using Xunit;

namespace PimFlow.Server.Tests.Common;

/// <summary>
/// Tests para las extensiones de ApiResponse que usan Result de Domain
/// Movidos desde PimFlow.Shared.Tests para mantener Clean Architecture
/// </summary>
public class ApiResponseExtensionsTests
{
    [Fact]
    public void ToApiResponse_SuccessfulResult_ShouldCreateSuccessfulApiResponse()
    {
        // Arrange
        var result = Result.Success("test data");

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().Be("test data");
        apiResponse.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ToApiResponse_FailedResult_ShouldCreateErrorApiResponse()
    {
        // Arrange
        var result = Result.Failure<string>("Something went wrong");

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.Data.Should().BeNull();
        apiResponse.ErrorMessage.Should().Be("Something went wrong");
    }

    [Fact]
    public void ToApiResponse_SuccessfulResultWithoutData_ShouldCreateSuccessfulApiResponse()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ToApiResponse_FailedResultWithoutData_ShouldCreateErrorApiResponse()
    {
        // Arrange
        var result = Result.Failure("Something went wrong");

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Something went wrong");
    }

    [Fact]
    public void ToApiResponse_ResultWithComplexData_ShouldPreserveData()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test", Items = new[] { "A", "B", "C" } };
        var result = Result.Success(testData);

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(testData);
        apiResponse.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ToApiResponse_ResultWithNullData_ShouldHandleCorrectly()
    {
        // Arrange
        string? nullData = null;
        var result = Result.Success(nullData);

        // Act
        var apiResponse = result.ToApiResponse();

        // Assert
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeNull();
        apiResponse.ErrorMessage.Should().BeNull();
    }
}
