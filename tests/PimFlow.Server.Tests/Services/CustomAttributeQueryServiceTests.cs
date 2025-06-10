using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Enums;
using PimFlow.Server.Services;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests for CustomAttributeQueryService (CQRS Query side)
/// Tests the actual business logic for read operations
/// </summary>
public class CustomAttributeQueryServiceTests
{
    private readonly Mock<ICustomAttributeRepository> _mockRepository;
    private readonly CustomAttributeQueryService _service;

    public CustomAttributeQueryServiceTests()
    {
        _mockRepository = new Mock<ICustomAttributeRepository>();
        _service = new CustomAttributeQueryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAttributesAsync_ShouldReturnAllAttributes()
    {
        // Arrange
        var attributes = new List<CustomAttribute>
        {
            new() { Id = 1, Name = "color", DisplayName = "Color", Type = AttributeType.Color, IsActive = true },
            new() { Id = 2, Name = "size", DisplayName = "Talle", Type = AttributeType.Select, IsActive = false }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(attributes);

        // Act
        var result = await _service.GetAllAttributesAsync();

        // Assert
        result.Should().HaveCount(2);
        var resultList = result.ToList();
        resultList[0].Name.Should().Be("color");
        resultList[1].Name.Should().Be("size");
    }

    [Fact]
    public async Task GetActiveAttributesAsync_ShouldReturnOnlyActiveAttributes()
    {
        // Arrange
        var activeAttributes = new List<CustomAttribute>
        {
            new() { Id = 1, Name = "color", DisplayName = "Color", Type = AttributeType.Color, IsActive = true }
        };

        _mockRepository.Setup(x => x.GetActiveAsync())
            .ReturnsAsync(activeAttributes);

        // Act
        var result = await _service.GetActiveAttributesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("color");
        result.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetAttributeByIdAsync_ExistingId_ShouldReturnAttribute()
    {
        // Arrange
        var attribute = new CustomAttribute
        {
            Id = 1,
            Name = "material",
            DisplayName = "Material",
            Type = AttributeType.Text,
            IsActive = true
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(attribute);

        // Act
        var result = await _service.GetAttributeByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("material");
    }

    [Fact]
    public async Task GetAttributeByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((CustomAttribute?)null);

        // Act
        var result = await _service.GetAttributeByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
