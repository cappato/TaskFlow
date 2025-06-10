using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Enums;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests for CustomAttributeCommandService (CQRS Command side)
/// Tests the actual business logic for write operations
/// </summary>
public class CustomAttributeCommandServiceTests
{
    private readonly Mock<ICustomAttributeRepository> _mockRepository;
    private readonly CustomAttributeCommandService _service;

    public CustomAttributeCommandServiceTests()
    {
        _mockRepository = new Mock<ICustomAttributeRepository>();
        _service = new CustomAttributeCommandService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAttributeAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateCustomAttributeDto
        {
            Name = "material",
            DisplayName = "Material",
            Type = DomainEnumMapper.ToShared(AttributeType.Text),
            IsRequired = false,
            SortOrder = 3
        };

        var createdAttribute = new CustomAttribute
        {
            Id = 1,
            Name = createDto.Name,
            DisplayName = createDto.DisplayName,
            Type = DomainEnumMapper.ToDomain(createDto.Type),
            IsRequired = createDto.IsRequired,
            SortOrder = createDto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.ExistsByNameAsync(createDto.Name))
            .ReturnsAsync(false);

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<CustomAttribute>()))
            .ReturnsAsync(createdAttribute);

        // Act
        var result = await _service.CreateAttributeAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("material");
        result.DisplayName.Should().Be("Material");
        result.Type.Should().Be(DomainEnumMapper.ToShared(AttributeType.Text));
        result.IsActive.Should().BeTrue();

        _mockRepository.Verify(x => x.ExistsByNameAsync(createDto.Name), Times.Once);
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<CustomAttribute>()), Times.Once);
    }

    [Fact]
    public async Task CreateAttributeAsync_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateCustomAttributeDto
        {
            Name = "existing-attribute",
            DisplayName = "Existing Attribute",
            Type = DomainEnumMapper.ToShared(AttributeType.Text)
        };

        _mockRepository.Setup(x => x.ExistsByNameAsync(createDto.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAttributeAsync(createDto));

        exception.Message.Should().Contain("Ya existe un atributo con nombre: existing-attribute");
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<CustomAttribute>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAttributeAsync_ExistingAttribute_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingAttribute = new CustomAttribute
        {
            Id = 1,
            Name = "old-name",
            DisplayName = "Old Display Name",
            Type = AttributeType.Text,
            IsRequired = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateCustomAttributeDto
        {
            Name = "new-name",
            DisplayName = "New Display Name",
            Type = DomainEnumMapper.ToShared(AttributeType.Color),
            IsRequired = true
        };

        var updatedAttribute = new CustomAttribute
        {
            Id = 1,
            Name = updateDto.Name,
            DisplayName = updateDto.DisplayName,
            Type = DomainEnumMapper.ToDomain(updateDto.Type!.Value),
            IsRequired = updateDto.IsRequired!.Value,
            IsActive = true,
            CreatedAt = existingAttribute.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingAttribute);

        _mockRepository.Setup(x => x.ExistsByNameAsync(updateDto.Name))
            .ReturnsAsync(false);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<CustomAttribute>()))
            .ReturnsAsync(updatedAttribute);

        // Act
        var result = await _service.UpdateAttributeAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("new-name");
        result.DisplayName.Should().Be("New Display Name");
        result.Type.Should().Be(DomainEnumMapper.ToShared(AttributeType.Color));
        result.IsRequired.Should().BeTrue();

        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CustomAttribute>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAttributeAsync_NonExistingAttribute_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateCustomAttributeDto
        {
            Name = "new-name"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((CustomAttribute?)null);

        // Act
        var result = await _service.UpdateAttributeAsync(999, updateDto);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CustomAttribute>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAttributeAsync_ShouldCallRepository()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAttributeAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }
}
