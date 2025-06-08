using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Server.Repositories;
using FluentAssertions;
using Xunit;

namespace TaskFlow.Server.Tests.Repositories;

public class CustomAttributeRepositoryTests : IDisposable
{
    private readonly TaskFlowDbContext _context;
    private readonly CustomAttributeRepository _repository;

    public CustomAttributeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskFlowDbContext(options);
        _repository = new CustomAttributeRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var attributes = new[]
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
            },
            new CustomAttribute
            {
                Id = 3,
                Name = "waterproof",
                DisplayName = "Resistente al Agua",
                Type = AttributeType.Boolean,
                IsRequired = false,
                SortOrder = 3,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.CustomAttributes.AddRange(attributes);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAttributes()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeInAscendingOrder(x => x.SortOrder);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveAttributes()
    {
        // Act
        var result = await _repository.GetActiveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.IsActive);
        result.Should().BeInAscendingOrder(x => x.SortOrder);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnAttribute()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("color");
        result.DisplayName.Should().Be("Color");
        result.Type.Should().Be(AttributeType.Text);
        result.IsRequired.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithValidName_ShouldReturnAttribute()
    {
        // Act
        var result = await _repository.GetByNameAsync("color");

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Color");
    }

    [Fact]
    public async Task GetByNameAsync_WithInvalidName_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByNameAsync("invalid_name");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidAttribute_ShouldCreateSuccessfully()
    {
        // Arrange
        var newAttribute = new CustomAttribute
        {
            Name = "material",
            DisplayName = "Material",
            Type = AttributeType.Text,
            IsRequired = false,
            SortOrder = 4,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(newAttribute);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("material");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_WithValidAttribute_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingAttribute = await _repository.GetByIdAsync(1);
        existingAttribute!.DisplayName = "Updated Color";
        existingAttribute.IsRequired = false;

        // Act
        var result = await _repository.UpdateAsync(existingAttribute);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Updated Color");
        result.IsRequired.Should().BeFalse();
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidAttribute = new CustomAttribute
        {
            Id = 999,
            Name = "invalid",
            DisplayName = "Invalid",
            Type = AttributeType.Text
        };

        // Act
        var result = await _repository.UpdateAsync(invalidAttribute);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteSuccessfully()
    {
        // Act
        var result = await _repository.DeleteAsync(3);

        // Assert
        result.Should().BeTrue();

        var deletedAttribute = await _repository.GetByIdAsync(3);
        deletedAttribute.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_WithExistingName_ShouldReturnTrue()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("color");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_WithNonExistingName_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByNameAsync("non_existing");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderBySortOrderThenDisplayName()
    {
        // Arrange - Add attribute with same sort order to test secondary ordering
        var newAttribute = new CustomAttribute
        {
            Name = "brand",
            DisplayName = "Marca",
            Type = AttributeType.Text,
            SortOrder = 1, // Same as color
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(newAttribute);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        var resultList = result.ToList();

        // Should be ordered by SortOrder first, then by DisplayName
        resultList[0].SortOrder.Should().Be(1);
        resultList[1].SortOrder.Should().Be(1);
        resultList[0].DisplayName.Should().Be("Color"); // "Color" comes before "Marca" alphabetically
        resultList[1].DisplayName.Should().Be("Marca");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
