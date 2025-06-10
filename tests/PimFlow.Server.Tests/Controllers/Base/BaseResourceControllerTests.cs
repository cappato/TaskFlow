using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Server.Controllers.Base;
using PimFlow.Server.Services;
using PimFlow.Shared.Common;
using Xunit;

namespace PimFlow.Server.Tests.Controllers.Base;

/// <summary>
/// Tests para BaseResourceController
/// Verifica funcionalidad CRUD estándar
/// </summary>
public class BaseResourceControllerTests
{
    private readonly Mock<ITestService> _mockService;
    private readonly Mock<ILogger<TestResourceController>> _mockLogger;
    private readonly Mock<IDomainEventService> _mockDomainEventService;
    private readonly TestResourceController _controller;

    public BaseResourceControllerTests()
    {
        _mockService = new Mock<ITestService>();
        _mockLogger = new Mock<ILogger<TestResourceController>>();
        _mockDomainEventService = new Mock<IDomainEventService>();
        _controller = new TestResourceController(_mockService.Object, _mockLogger.Object, _mockDomainEventService.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllItems()
    {
        // Arrange
        var expectedItems = new List<TestDto>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" }
        };
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<TestDto>>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<TestDto>>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnItem()
    {
        // Arrange
        var itemId = 1;
        var expectedItem = new TestDto { Id = itemId, Name = "Test Item" };
        _mockService.Setup(x => x.GetByIdAsync(itemId)).ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.GetById(itemId);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<TestDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<TestDto>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedItem);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var itemId = 999;
        _mockService.Setup(x => x.GetByIdAsync(itemId)).ReturnsAsync((TestDto?)null);

        // Act
        var result = await _controller.GetById(itemId);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<TestDto>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<TestDto>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("TestDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedItem()
    {
        // Arrange
        var createDto = new CreateTestDto { Name = "New Item" };
        var createdItem = new TestDto { Id = 1, Name = "New Item" };
        _mockService.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(createdItem);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<TestDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<TestDto>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(createdItem);
    }

    [Fact]
    public async Task Update_WithValidIdAndDto_ShouldReturnUpdatedItem()
    {
        // Arrange
        var itemId = 1;
        var updateDto = new UpdateTestDto { Name = "Updated Item" };
        var updatedItem = new TestDto { Id = itemId, Name = "Updated Item" };
        _mockService.Setup(x => x.UpdateAsync(itemId, updateDto)).ReturnsAsync(updatedItem);

        // Act
        var result = await _controller.Update(itemId, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<TestDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<TestDto>>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(updatedItem);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var itemId = 999;
        var updateDto = new UpdateTestDto { Name = "Updated Item" };
        _mockService.Setup(x => x.UpdateAsync(itemId, updateDto)).ReturnsAsync((TestDto?)null);

        // Act
        var result = await _controller.Update(itemId, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<TestDto>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<TestDto>>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("TestDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var itemId = 1;
        _mockService.Setup(x => x.DeleteAsync(itemId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(itemId);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse>().Subject;
        
        apiResponse.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var itemId = 999;
        _mockService.Setup(x => x.DeleteAsync(itemId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(itemId);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse>().Subject;
        
        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("TestDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public void GetItemId_WithValidItem_ShouldReturnId()
    {
        // Arrange
        var item = new TestDto { Id = 123, Name = "Test" };

        // Act
        var result = _controller.TestGetItemId(item);

        // Assert
        result.Should().Be(123);
    }
}

/// <summary>
/// Controlador de prueba que hereda de BaseResourceController
/// </summary>
public class TestResourceController : BaseResourceController<TestDto, CreateTestDto, UpdateTestDto, ITestService>
{
    public TestResourceController(ITestService service, ILogger<TestResourceController> logger, IDomainEventService? domainEventService = null)
        : base(service, logger, domainEventService)
    {
    }

    protected override async Task<IEnumerable<TestDto>> GetAllItemsAsync()
    {
        return await Service.GetAllAsync();
    }

    protected override async Task<TestDto?> GetItemByIdAsync(int id)
    {
        return await Service.GetByIdAsync(id);
    }

    protected override async Task<TestDto> CreateItemAsync(CreateTestDto createDto)
    {
        return await Service.CreateAsync(createDto);
    }

    protected override async Task<TestDto?> UpdateItemAsync(int id, UpdateTestDto updateDto)
    {
        return await Service.UpdateAsync(id, updateDto);
    }

    protected override async Task<bool> DeleteItemAsync(int id)
    {
        return await Service.DeleteAsync(id);
    }

    // Exponer método protegido para testing
    public object? TestGetItemId(TestDto item) => GetItemId(item);
}

/// <summary>
/// Interface de servicio de prueba
/// </summary>
public interface ITestService
{
    Task<IEnumerable<TestDto>> GetAllAsync();
    Task<TestDto?> GetByIdAsync(int id);
    Task<TestDto> CreateAsync(CreateTestDto createDto);
    Task<TestDto?> UpdateAsync(int id, UpdateTestDto updateDto);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// DTO de prueba
/// </summary>
public class TestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO de creación de prueba
/// </summary>
public class CreateTestDto
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO de actualización de prueba
/// </summary>
public class UpdateTestDto
{
    public string Name { get; set; } = string.Empty;
}
