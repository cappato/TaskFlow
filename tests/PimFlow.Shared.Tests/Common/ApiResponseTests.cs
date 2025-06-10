using FluentAssertions;
using PimFlow.Shared.Common;
using Xunit;

namespace PimFlow.Shared.Tests.Common;

public class ApiResponseTests
{
    [Fact]
    public void Success_WithData_ShouldCreateSuccessfulResponse()
    {
        // Arrange
        var data = "test data";

        // Act
        var response = ApiResponse<string>.Success(data);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(data);
        response.ErrorMessage.Should().BeNull();
        response.ValidationErrors.Should().BeEmpty();
        response.ErrorCode.Should().BeNull();
        response.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Success_WithoutData_ShouldCreateSuccessfulResponse()
    {
        // Act
        var response = ApiResponse<string>.Success();

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeNull();
        response.ErrorMessage.Should().BeNull();
        response.ValidationErrors.Should().BeEmpty();
        response.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void Error_WithMessage_ShouldCreateErrorResponse()
    {
        // Arrange
        var errorMessage = "Something went wrong";
        var errorCode = "TEST_ERROR";

        // Act
        var response = ApiResponse<string>.Error(errorMessage, errorCode);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.ErrorMessage.Should().Be(errorMessage);
        response.ErrorCode.Should().Be(errorCode);
        response.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationError_WithList_ShouldCreateValidationErrorResponse()
    {
        // Arrange
        var validationErrors = new List<string> { "Field1 is required", "Field2 is invalid" };

        // Act
        var response = ApiResponse<string>.ValidationError(validationErrors);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.ErrorMessage.Should().Be("Errores de validación");
        response.ErrorCode.Should().Be("VALIDATION_ERROR");
        response.ValidationErrors.Should().BeEquivalentTo(validationErrors);
    }

    [Fact]
    public void ValidationError_WithSingleError_ShouldCreateValidationErrorResponse()
    {
        // Arrange
        var validationError = "Field is required";

        // Act
        var response = ApiResponse<string>.ValidationError(validationError);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be("Errores de validación");
        response.ErrorCode.Should().Be("VALIDATION_ERROR");
        response.ValidationErrors.Should().Contain(validationError);
        response.ValidationErrors.Should().HaveCount(1);
    }

    [Fact]
    public void NotFound_WithMessage_ShouldCreateNotFoundResponse()
    {
        // Arrange
        var message = "Resource not found";

        // Act
        var response = ApiResponse<string>.NotFound(message);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be(message);
        response.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void NotFound_WithoutMessage_ShouldCreateNotFoundResponseWithDefaultMessage()
    {
        // Act
        var response = ApiResponse<string>.NotFound();

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be("Recurso no encontrado");
        response.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void Unauthorized_WithMessage_ShouldCreateUnauthorizedResponse()
    {
        // Arrange
        var message = "Access denied";

        // Act
        var response = ApiResponse<string>.Unauthorized(message);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be(message);
        response.ErrorCode.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public void Unauthorized_WithoutMessage_ShouldCreateUnauthorizedResponseWithDefaultMessage()
    {
        // Act
        var response = ApiResponse<string>.Unauthorized();

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be("Acceso no autorizado");
        response.ErrorCode.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public void InternalError_WithMessage_ShouldCreateInternalErrorResponse()
    {
        // Arrange
        var message = "Database connection failed";

        // Act
        var response = ApiResponse<string>.InternalError(message);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be(message);
        response.ErrorCode.Should().Be("INTERNAL_ERROR");
    }

    [Fact]
    public void InternalError_WithoutMessage_ShouldCreateInternalErrorResponseWithDefaultMessage()
    {
        // Act
        var response = ApiResponse<string>.InternalError();

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be("Error interno del servidor");
        response.ErrorCode.Should().Be("INTERNAL_ERROR");
    }

    [Fact]
    public void HasValidationErrors_WithValidationErrors_ShouldReturnTrue()
    {
        // Arrange
        var response = ApiResponse<string>.ValidationError(new List<string> { "Error1", "Error2" });

        // Act & Assert
        response.HasValidationErrors.Should().BeTrue();
    }

    [Fact]
    public void HasValidationErrors_WithoutValidationErrors_ShouldReturnFalse()
    {
        // Arrange
        var response = ApiResponse<string>.Error("General error");

        // Act & Assert
        response.HasValidationErrors.Should().BeFalse();
    }

    [Fact]
    public void GetAllErrors_WithBothErrorMessageAndValidationErrors_ShouldReturnAll()
    {
        // Arrange
        var response = new ApiResponse<string>
        {
            IsSuccess = false,
            ErrorMessage = "General error",
            ValidationErrors = new List<string> { "Validation error 1", "Validation error 2" }
        };

        // Act
        var allErrors = response.GetAllErrors();

        // Assert
        allErrors.Should().HaveCount(3);
        allErrors.Should().Contain("General error");
        allErrors.Should().Contain("Validation error 1");
        allErrors.Should().Contain("Validation error 2");
    }

    [Fact]
    public void GetAllErrors_WithOnlyErrorMessage_ShouldReturnErrorMessage()
    {
        // Arrange
        var response = ApiResponse<string>.Error("General error");

        // Act
        var allErrors = response.GetAllErrors();

        // Assert
        allErrors.Should().HaveCount(1);
        allErrors.Should().Contain("General error");
    }

    [Fact]
    public void GetErrorsAsString_WithMultipleErrors_ShouldJoinWithSeparator()
    {
        // Arrange
        var response = new ApiResponse<string>
        {
            IsSuccess = false,
            ErrorMessage = "General error",
            ValidationErrors = new List<string> { "Validation error 1", "Validation error 2" }
        };

        // Act
        var errorsString = response.GetErrorsAsString(" | ");

        // Assert
        errorsString.Should().Be("General error | Validation error 1 | Validation error 2");
    }

    [Fact]
    public void GetErrorsAsString_WithDefaultSeparator_ShouldUseDefaultSeparator()
    {
        // Arrange
        var response = new ApiResponse<string>
        {
            IsSuccess = false,
            ErrorMessage = "Error 1",
            ValidationErrors = new List<string> { "Error 2" }
        };

        // Act
        var errorsString = response.GetErrorsAsString();

        // Assert
        errorsString.Should().Be("Error 1; Error 2");
    }

    #region ApiResponse (without generic) Tests

    [Fact]
    public void ApiResponse_Success_ShouldCreateSuccessfulResponse()
    {
        // Act
        var response = ApiResponse.Success();

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeNull();
        response.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void ApiResponse_Error_ShouldCreateErrorResponse()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var response = ApiResponse.Error(errorMessage);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ApiResponse_ValidationError_ShouldCreateValidationErrorResponse()
    {
        // Arrange
        var validationErrors = new List<string> { "Field is required" };

        // Act
        var response = ApiResponse.ValidationError(validationErrors);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be("VALIDATION_ERROR");
        response.ValidationErrors.Should().BeEquivalentTo(validationErrors);
    }

    #endregion


}
