using Moq;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Domain.Enums;
using TaskFlow.Server.Services;
using TaskFlow.Shared.DTOs;
using FluentAssertions;
using Xunit;

namespace TaskFlow.Server.Tests.Services;

public class CustomAttributeServiceTests
{
    private readonly Mock<ICustomAttributeRepository> _mockRepository;
    private readonly CustomAttributeService _service;

    public CustomAttributeServiceTests()
    {
        _mockRepository = new Mock<ICustomAttributeRepository>();
        _service = new CustomAttributeService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAttributesAsync_ShouldReturnMappedAttributes()
    {
        // Arrange
        var attributes = new List<CustomAttribute>
        {
            new CustomAttribute
            {
                Id = 1,
                Name = "color",
                DisplayName = "Color",
                Type = AttributeType.Text,
                IsRequired = true,
                SortOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new CustomAttribute
            {
                Id = 2,
                Name = "size",
                DisplayName = "Talle",
                Type = AttributeType.Text,
                IsRequired = false,
                SortOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(attributes);

        // Act
        var result = await _service.GetAllAttributesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var firstAttribute = result.First();
        firstAttribute.Name.Should().Be("color");
        firstAttribute.DisplayName.Should().Be("Color");
        firstAttribute.Type.Should().Be(AttributeType.Text);
        firstAttribute.IsRequired.Should().BeTrue();
    }

    [Fact]
    public async Task GetActiveAttributesAsync_ShouldReturnOnlyActiveAttributes()
    {
        // Arrange
        var activeAttributes = new List<CustomAttribute>
        {
            new CustomAttribute
            {
                Id = 1,
                Name = "color",
                DisplayName = "Color",
                Type = AttributeType.Text,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(x => x.GetActiveAsync())
            .ReturnsAsync(activeAttributes);

        // Act
        var result = await _service.GetActiveAttributesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().OnlyContain(x => x.IsActive);

        _mockRepository.Verify(x => x.GetActiveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAttributeByIdAsync_WithValidId_ShouldReturnMappedAttribute()
    {
        // Arrange
        var attribute = new CustomAttribute
        {
            Id = 1,
            Name = "color",
            DisplayName = "Color",
            Type = AttributeType.Text,
            IsRequired = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(attribute);

        // Act
        var result = await _service.GetAttributeByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("color");
        result.DisplayName.Should().Be("Color");
    }

    [Fact]
    public async Task GetAttributeByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((CustomAttribute?)null);

        // Act
        var result = await _service.GetAttributeByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAttributeAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateCustomAttributeDto
        {
            Name = "material",
            DisplayName = "Material",
            Type = AttributeType.Text,
            IsRequired = false,
            SortOrder = 3
        };

        var createdAttribute = new CustomAttribute
        {
            Id = 1,
            Name = createDto.Name,
            DisplayName = createDto.DisplayName,
            Type = createDto.Type,
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
        result.Type.Should().Be(AttributeType.Text);
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
            Name = "existing_attribute",
            DisplayName = "Existing Attribute"
        };

        _mockRepository.Setup(x => x.ExistsByNameAsync(createDto.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAttributeAsync(createDto));

        exception.Message.Should().Contain("Ya existe un atributo con nombre: existing_attribute");

        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<CustomAttribute>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAttributeAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingAttribute = new CustomAttribute
        {
            Id = 1,
            Name = "color",
            DisplayName = "Color",
            Type = AttributeType.Text,
            IsRequired = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var updateDto = new UpdateCustomAttributeDto
        {
            DisplayName = "Updated Color",
            IsRequired = true,
            Type = AttributeType.Color
        };

        var updatedAttribute = new CustomAttribute
        {
            Id = 1,
            Name = "color",
            DisplayName = "Updated Color",
            Type = AttributeType.Color,
            IsRequired = true,
            CreatedAt = existingAttribute.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingAttribute);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<CustomAttribute>()))
            .ReturnsAsync(updatedAttribute);

        // Act
        var result = await _service.UpdateAttributeAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Updated Color");
        result.IsRequired.Should().BeTrue();
        result.Type.Should().Be(AttributeType.Color);

        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CustomAttribute>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAttributeAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateCustomAttributeDto { DisplayName = "Updated" };

        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((CustomAttribute?)null);

        // Act
        var result = await _service.UpdateAttributeAsync(999, updateDto);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CustomAttribute>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAttributeAsync_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var existingAttribute = new CustomAttribute
        {
            Id = 1,
            Name = "color",
            DisplayName = "Color",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var updateDto = new UpdateCustomAttributeDto
        {
            Name = "existing_name"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingAttribute);

        _mockRepository.Setup(x => x.ExistsByNameAsync("existing_name"))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAttributeAsync(1, updateDto));

        exception.Message.Should().Contain("Ya existe un atributo con nombre: existing_name");

        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CustomAttribute>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAttributeAsync_WithValidId_ShouldReturnTrue()
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

    [Fact]
    public async Task DeleteAttributeAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAttributeAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.DeleteAsync(999), Times.Once);
    }
}
