using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Server.Controllers.Base;
using PimFlow.Server.Services;
using PimFlow.Shared.Common;
using PimFlow.Contracts.Common;
using PimFlow.Domain.Common;
using Xunit;

namespace PimFlow.Server.Tests.Controllers.Base;

/// <summary>
/// Tests para BaseApiController
/// Verifica funcionalidad común de controladores API
/// </summary>
public class BaseApiControllerTests
{
    private readonly Mock<ILogger<TestController>> _mockLogger;
    private readonly Mock<IDomainEventService> _mockDomainEventService;
    private readonly TestController _controller;

    public BaseApiControllerTests()
    {
        _mockLogger = new Mock<ILogger<TestController>>();
        _mockDomainEventService = new Mock<IDomainEventService>();
        _controller = new TestController(_mockLogger.Object, _mockDomainEventService.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Act & Assert
        _controller.Should().NotBeNull();
        _controller.Logger.Should().Be(_mockLogger.Object);
        _controller.DomainEventService.Should().Be(_mockDomainEventService.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new TestController(null!, _mockDomainEventService.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void SuccessResponse_WithData_ShouldReturnOkWithApiResponse()
    {
        // Arrange
        var testData = "test data";

        // Act
        var result = _controller.TestSuccessResponse(testData);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().Be(testData);
        apiResponse.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void SuccessResponse_WithoutData_ShouldReturnOkWithApiResponse()
    {
        // Act
        var result = _controller.TestSuccessResponseWithoutData();

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ErrorResponse_WithMessage_ShouldReturnBadRequestWithApiResponse()
    {
        // Arrange
        var errorMessage = "Test error";
        var errorCode = "TEST_ERROR";

        // Act
        var result = _controller.TestErrorResponse<string>(errorMessage, errorCode);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be(errorMessage);
        apiResponse.ErrorCode.Should().Be(errorCode);
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public void NotFoundResponse_WithMessage_ShouldReturnNotFoundWithApiResponse()
    {
        // Arrange
        var message = "Resource not found";

        // Act
        var result = _controller.TestNotFoundResponse<string>(message);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be(message);
        apiResponse.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void ValidationErrorResponse_WithErrors_ShouldReturnBadRequestWithApiResponse()
    {
        // Arrange
        var validationErrors = new[] { "Error 1", "Error 2" };

        // Act
        var result = _controller.TestValidationErrorResponse<string>(validationErrors);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Errores de validación");
        apiResponse.ErrorCode.Should().Be("VALIDATION_ERROR");
        apiResponse.ValidationErrors.Should().BeEquivalentTo(validationErrors);
    }

    [Fact]
    public void ValidateStringParameter_WithValidValue_ShouldReturnNull()
    {
        // Arrange
        var validValue = "valid value";
        var parameterName = "testParam";

        // Act
        var result = _controller.TestValidateStringParameter<string>(validValue, parameterName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateStringParameter_WithNullValue_ShouldReturnValidationError()
    {
        // Arrange
        string? nullValue = null;
        var parameterName = "testParam";

        // Act
        var result = _controller.TestValidateStringParameter<string>(nullValue, parameterName);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result!.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ValidationErrors.Should().Contain($"{parameterName} es requerido y no puede estar vacío");
    }

    [Fact]
    public void ValidateStringParameter_WithEmptyValue_ShouldReturnValidationError()
    {
        // Arrange
        var emptyValue = "";
        var parameterName = "testParam";

        // Act
        var result = _controller.TestValidateStringParameter<string>(emptyValue, parameterName);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result!.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ValidationErrors.Should().Contain($"{parameterName} es requerido y no puede estar vacío");
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulOperation_ShouldReturnSuccessResponse()
    {
        // Arrange
        var expectedResult = "success";
        Func<Task<string>> operation = () => Task.FromResult(expectedResult);

        // Act
        var result = await _controller.TestExecuteAsync(operation, "TestOperation");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidOperationException_ShouldReturnErrorResponse()
    {
        // Arrange
        var errorMessage = "Invalid operation";
        Func<Task<string>> operation = () => throw new InvalidOperationException(errorMessage);

        // Act
        var result = await _controller.TestExecuteAsync(operation, "TestOperation");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be(errorMessage);
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task ExecuteAsync_WithArgumentException_ShouldReturnErrorResponse()
    {
        // Arrange
        var errorMessage = "Invalid argument";
        Func<Task<string>> operation = () => throw new ArgumentException(errorMessage);

        // Act
        var result = await _controller.TestExecuteAsync(operation, "TestOperation");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be(errorMessage);
        apiResponse.ErrorCode.Should().Be("INVALID_ARGUMENT");
    }

    [Fact]
    public async Task ExecuteAsync_WithUnexpectedException_ShouldReturnInternalErrorResponse()
    {
        // Arrange
        Func<Task<string>> operation = () => throw new Exception("Unexpected error");

        // Act
        var result = await _controller.TestExecuteAsync(operation, "TestOperation");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Error interno del servidor");
        apiResponse.ErrorCode.Should().Be("INTERNAL_ERROR");
    }

    [Fact]
    public void ToActionResult_WithSuccessfulResult_ShouldReturnSuccessResponse()
    {
        // Arrange
        var data = "test data";
        var result = Result.Success(data);

        // Act
        var actionResult = _controller.TestToActionResult(result);

        // Assert
        actionResult.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().Be(data);
    }

    [Fact]
    public void ToActionResult_WithFailedResult_ShouldReturnErrorResponse()
    {
        // Arrange
        var errorMessage = "Operation failed";
        var result = Result.Failure<string>(errorMessage);

        // Act
        var actionResult = _controller.TestToActionResult(result);

        // Assert
        actionResult.Should().BeOfType<ActionResult<ApiResponse<string>>>();
        var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<string>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public async Task PublishDomainEventsAsync_WithEventsAndService_ShouldCallService()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot();
        aggregateRoot.AddDomainEvent(new TestDomainEvent());

        // Act
        await _controller.TestPublishDomainEventsAsync(aggregateRoot);

        // Assert
        _mockDomainEventService.Verify(
            x => x.PublishEventsAsync(aggregateRoot, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task PublishDomainEventsAsync_WithoutService_ShouldNotThrow()
    {
        // Arrange
        var controllerWithoutService = new TestController(_mockLogger.Object, null);
        var aggregateRoot = new TestAggregateRoot();
        aggregateRoot.AddDomainEvent(new TestDomainEvent());

        // Act & Assert
        var act = async () => await controllerWithoutService.TestPublishDomainEventsAsync(aggregateRoot);
        await act.Should().NotThrowAsync();
    }
}

/// <summary>
/// Controlador de prueba que expone métodos protegidos para testing
/// </summary>
public class TestController : BaseApiController
{
    public TestController(ILogger<TestController> logger, IDomainEventService? domainEventService = null)
        : base(logger, domainEventService)
    {
    }

    // Exponer métodos protegidos para testing
    public new ILogger Logger => base.Logger;
    public new IDomainEventService? DomainEventService => base.DomainEventService;

    public ActionResult<ApiResponse<T>> TestSuccessResponse<T>(T data) => SuccessResponse(data);
    public ActionResult<ApiResponse> TestSuccessResponseWithoutData() => SuccessResponse();
    public ActionResult<ApiResponse<T>> TestErrorResponse<T>(string errorMessage, string? errorCode = null) => ErrorResponse<T>(errorMessage, errorCode);
    public ActionResult<ApiResponse<T>> TestNotFoundResponse<T>(string? message = null) => NotFoundResponse<T>(message);
    public ActionResult<ApiResponse<T>> TestValidationErrorResponse<T>(IEnumerable<string> validationErrors) => ValidationErrorResponse<T>(validationErrors);
    public ActionResult<ApiResponse<T>>? TestValidateStringParameter<T>(string? value, string parameterName) => ValidateStringParameter<T>(value, parameterName);
    public Task<ActionResult<ApiResponse<T>>> TestExecuteAsync<T>(Func<Task<T>> operation, string operationName) => ExecuteAsync(operation, operationName);
    public ActionResult<ApiResponse<T>> TestToActionResult<T>(IResult<T> result) => ToActionResult(result);
    public Task TestPublishDomainEventsAsync(AggregateRoot aggregateRoot) => PublishDomainEventsAsync(aggregateRoot);
}

/// <summary>
/// Aggregate root de prueba para testing
/// </summary>
public class TestAggregateRoot : AggregateRoot
{
    public new void AddDomainEvent(IDomainEvent domainEvent)
    {
        base.AddDomainEvent(domainEvent);
    }
}

/// <summary>
/// Evento de dominio de prueba para testing
/// </summary>
public class TestDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventName => "TestEvent";
    public int Version => 1;
}
