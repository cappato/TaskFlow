using Moq;
using PimFlow.Domain.Article;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Article.Enums;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Services;

public class CustomAttributeServiceTests
{
    private readonly Mock<ICustomAttributeQueryService> _mockQueryService;
    private readonly Mock<ICustomAttributeCommandService> _mockCommandService;
    private readonly CustomAttributeService _service;

    public CustomAttributeServiceTests()
    {
        _mockQueryService = new Mock<ICustomAttributeQueryService>();
        _mockCommandService = new Mock<ICustomAttributeCommandService>();
        _service = new CustomAttributeService(_mockQueryService.Object, _mockCommandService.Object);
    }

    [Fact]
    public async Task GetAllAttributesAsync_ShouldDelegateToQueryService()
    {
        // Arrange
        var expectedAttributes = new List<CustomAttributeDto>
        {
            new() { Id = 1, Name = "color" },
            new() { Id = 2, Name = "size" }
        };

        _mockQueryService.Setup(x => x.GetAllAttributesAsync())
            .ReturnsAsync(expectedAttributes);

        // Act
        var result = await _service.GetAllAttributesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedAttributes);
        _mockQueryService.Verify(x => x.GetAllAttributesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetActiveAttributesAsync_ShouldDelegateToQueryService()
    {
        // Arrange
        var expectedAttributes = new List<CustomAttributeDto>
        {
            new() { Id = 1, Name = "color", IsActive = true }
        };

        _mockQueryService.Setup(x => x.GetActiveAttributesAsync())
            .ReturnsAsync(expectedAttributes);

        // Act
        var result = await _service.GetActiveAttributesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedAttributes);
        _mockQueryService.Verify(x => x.GetActiveAttributesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAttributeByIdAsync_ShouldDelegateToQueryService()
    {
        // Arrange
        var expectedAttribute = new CustomAttributeDto { Id = 1, Name = "material" };

        _mockQueryService.Setup(x => x.GetAttributeByIdAsync(1))
            .ReturnsAsync(expectedAttribute);

        // Act
        var result = await _service.GetAttributeByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedAttribute);
        _mockQueryService.Verify(x => x.GetAttributeByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateAttributeAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        var createDto = new CreateCustomAttributeDto { Name = "new-attribute" };
        var expectedResult = new CustomAttributeDto { Id = 1, Name = "new-attribute" };

        _mockCommandService.Setup(x => x.CreateAttributeAsync(createDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.CreateAttributeAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockCommandService.Verify(x => x.CreateAttributeAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task UpdateAttributeAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        var updateDto = new UpdateCustomAttributeDto { Name = "updated-attribute" };
        var expectedResult = new CustomAttributeDto { Id = 1, Name = "updated-attribute" };

        _mockCommandService.Setup(x => x.UpdateAttributeAsync(1, updateDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.UpdateAttributeAsync(1, updateDto);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockCommandService.Verify(x => x.UpdateAttributeAsync(1, updateDto), Times.Once);
    }

    [Fact]
    public async Task DeleteAttributeAsync_ShouldDelegateToCommandService()
    {
        // Arrange
        _mockCommandService.Setup(x => x.DeleteAttributeAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAttributeAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockCommandService.Verify(x => x.DeleteAttributeAsync(1), Times.Once);
    }
}
