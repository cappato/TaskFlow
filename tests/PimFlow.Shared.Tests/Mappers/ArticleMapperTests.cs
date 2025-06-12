using FluentAssertions;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Enums;
using PimFlow.Shared.Mappers;
using PimFlow.Shared.ViewModels;
using Xunit;

namespace PimFlow.Shared.Tests.Mappers;

public class ArticleMapperTests
{
    private ArticleDto CreateSampleArticleDto()
    {
        return new ArticleDto
        {
            Id = 1,
            SKU = "ABC123",
            Name = "Test Product",
            Description = "Test Description",
            Type = ArticleType.Footwear,
            Brand = "Test Brand",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2),
            IsActive = true,
            CategoryId = 10,
            CategoryName = "Test Category",
            SupplierId = 20,
            SupplierName = "Test Supplier",
            CustomAttributes = new Dictionary<string, object>
            {
                { "Color", "Red" },
                { "Size", "42" }
            }
        };
    }

    private CreateArticleViewModel CreateSampleCreateViewModel()
    {
        return new CreateArticleViewModel
        {
            SKU = "XYZ789",
            Name = "New Product",
            Description = "New Description",
            Type = "Clothing",
            Brand = "New Brand",
            CategoryId = 15,
            SupplierId = 25,
            CustomAttributes = new Dictionary<string, object>
            {
                { "Material", "Cotton" },
                { "Weight", "500g" }
            }
        };
    }

    [Fact]
    public void ToViewModel_ValidDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = CreateSampleArticleDto();

        // Act
        var viewModel = ArticleMapper.ToViewModel(dto);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.Id.Should().Be(dto.Id);
        viewModel.SKU.Should().Be(dto.SKU);
        viewModel.Name.Should().Be(dto.Name);
        viewModel.Description.Should().Be(dto.Description);
        viewModel.Type.Should().Be(dto.Type.ToString());
        viewModel.Brand.Should().Be(dto.Brand);
        viewModel.CreatedAt.Should().Be(dto.CreatedAt);
        viewModel.UpdatedAt.Should().Be(dto.UpdatedAt);
        viewModel.IsActive.Should().Be(dto.IsActive);
        viewModel.CategoryId.Should().Be(dto.CategoryId);
        viewModel.CategoryName.Should().Be(dto.CategoryName);
        viewModel.SupplierId.Should().Be(dto.SupplierId);
        viewModel.SupplierName.Should().Be(dto.SupplierName);
        viewModel.CustomAttributes.Should().BeEquivalentTo(dto.CustomAttributes);
    }

    [Fact]
    public void ToViewModel_CalculatedProperties_ShouldWork()
    {
        // Arrange
        var dto = CreateSampleArticleDto();

        // Act
        var viewModel = ArticleMapper.ToViewModel(dto);

        // Assert
        viewModel.DisplayName.Should().Be("Test Brand - Test Product");
        viewModel.StatusText.Should().Be("Activo");
        viewModel.CreatedAtFormatted.Should().Be("01/01/2024");
        viewModel.UpdatedAtFormatted.Should().Be("02/01/2024");
        viewModel.IsValidForDisplay.Should().BeTrue();
    }

    [Fact]
    public void ToViewModelList_ValidDtos_ShouldMapAll()
    {
        // Arrange
        var dto2 = CreateSampleArticleDto();
        dto2.Id = 2;
        dto2.SKU = "DEF456";

        var dtos = new List<ArticleDto>
        {
            CreateSampleArticleDto(),
            dto2
        };

        // Act
        var viewModels = ArticleMapper.ToViewModelList(dtos);

        // Assert
        viewModels.Should().HaveCount(2);
        viewModels[0].Id.Should().Be(1);
        viewModels[1].Id.Should().Be(2);
        viewModels[1].SKU.Should().Be("DEF456");
    }

    [Fact]
    public void ToCreateDto_ValidViewModel_ShouldMapCorrectly()
    {
        // Arrange
        var viewModel = CreateSampleCreateViewModel();

        // Act
        var dto = ArticleMapper.ToCreateDto(viewModel);

        // Assert
        dto.Should().NotBeNull();
        dto.SKU.Should().Be(viewModel.SKU);
        dto.Name.Should().Be(viewModel.Name);
        dto.Description.Should().Be(viewModel.Description);
        dto.Type.Should().Be(ArticleType.Clothing);
        dto.Brand.Should().Be(viewModel.Brand);
        dto.CategoryId.Should().Be(viewModel.CategoryId);
        dto.SupplierId.Should().Be(viewModel.SupplierId);
        dto.CustomAttributes.Should().BeEquivalentTo(viewModel.CustomAttributes);
    }

    [Fact]
    public void ToCreateDto_TrimsWhitespace_ShouldWork()
    {
        // Arrange
        var viewModel = CreateSampleCreateViewModel();
        viewModel.SKU = "  XYZ789  ";
        viewModel.Name = "  New Product  ";
        viewModel.Brand = "  New Brand  ";
        viewModel.Description = "  New Description  ";

        // Act
        var dto = ArticleMapper.ToCreateDto(viewModel);

        // Assert
        dto.SKU.Should().Be("XYZ789");
        dto.Name.Should().Be("New Product");
        dto.Brand.Should().Be("New Brand");
        dto.Description.Should().Be("New Description");
    }

    [Fact]
    public void ToUpdateDto_ValidViewModel_ShouldMapCorrectly()
    {
        // Arrange
        var viewModel = new UpdateArticleViewModel
        {
            Id = 1,
            SKU = "UPD123",
            Name = "Updated Product",
            Description = "Updated Description",
            Type = "Accessory",
            Brand = "Updated Brand",
            CategoryId = 30,
            SupplierId = 40,
            CustomAttributes = new Dictionary<string, object> { { "Updated", "True" } }
        };

        // Act
        var dto = ArticleMapper.ToUpdateDto(viewModel);

        // Assert
        dto.Should().NotBeNull();
        dto.SKU.Should().Be(viewModel.SKU);
        dto.Name.Should().Be(viewModel.Name);
        dto.Description.Should().Be(viewModel.Description);
        dto.Type.Should().Be(ArticleType.Accessory);
        dto.Brand.Should().Be(viewModel.Brand);
        dto.CategoryId.Should().Be(viewModel.CategoryId);
        dto.SupplierId.Should().Be(viewModel.SupplierId);
        dto.CustomAttributes.Should().BeEquivalentTo(viewModel.CustomAttributes);
    }

    [Fact]
    public void ToCreateViewModel_NullDto_ShouldReturnEmptyViewModel()
    {
        // Act
        var viewModel = ArticleMapper.ToCreateViewModel(null);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.SKU.Should().BeEmpty();
        viewModel.Name.Should().BeEmpty();
        viewModel.Brand.Should().BeEmpty();
        viewModel.Type.Should().Be("Footwear"); // Default value
    }

    [Fact]
    public void ToCreateViewModel_ValidDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = CreateSampleArticleDto();

        // Act
        var viewModel = ArticleMapper.ToCreateViewModel(dto);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.SKU.Should().Be(dto.SKU);
        viewModel.Name.Should().Be(dto.Name);
        viewModel.Description.Should().Be(dto.Description);
        viewModel.Type.Should().Be(dto.Type.ToString());
        viewModel.Brand.Should().Be(dto.Brand);
        viewModel.CategoryId.Should().Be(dto.CategoryId);
        viewModel.SupplierId.Should().Be(dto.SupplierId);
        viewModel.CustomAttributes.Should().BeEquivalentTo(dto.CustomAttributes);
    }

    [Fact]
    public void ToUpdateViewModel_ValidDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = CreateSampleArticleDto();

        // Act
        var viewModel = ArticleMapper.ToUpdateViewModel(dto);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.Id.Should().Be(dto.Id);
        viewModel.SKU.Should().Be(dto.SKU);
        viewModel.Name.Should().Be(dto.Name);
        viewModel.Description.Should().Be(dto.Description);
        viewModel.Type.Should().Be(dto.Type.ToString());
        viewModel.Brand.Should().Be(dto.Brand);
        viewModel.CategoryId.Should().Be(dto.CategoryId);
        viewModel.SupplierId.Should().Be(dto.SupplierId);
        viewModel.CustomAttributes.Should().BeEquivalentTo(dto.CustomAttributes);
        viewModel.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void CopyToUpdateViewModel_ShouldCopyAllProperties()
    {
        // Arrange
        var source = ArticleMapper.ToViewModel(CreateSampleArticleDto());
        var target = new UpdateArticleViewModel();

        // Act
        ArticleMapper.CopyToUpdateViewModel(source, target);

        // Assert
        target.Id.Should().Be(source.Id);
        target.SKU.Should().Be(source.SKU);
        target.Name.Should().Be(source.Name);
        target.Description.Should().Be(source.Description);
        target.Type.Should().Be(source.Type);
        target.Brand.Should().Be(source.Brand);
        target.CategoryId.Should().Be(source.CategoryId);
        target.SupplierId.Should().Be(source.SupplierId);
        target.CustomAttributes.Should().BeEquivalentTo(source.CustomAttributes);
        target.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void ValidateForMapping_CreateViewModel_ValidData_ShouldReturnTrue()
    {
        // Arrange
        var viewModel = CreateSampleCreateViewModel();

        // Act
        var isValid = ArticleMapper.ValidateForMapping(viewModel, out var errors);

        // Assert
        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateForMapping_CreateViewModel_InvalidData_ShouldReturnFalse()
    {
        // Arrange
        var viewModel = new CreateArticleViewModel
        {
            SKU = "", // Invalid
            Name = "", // Invalid
            Brand = "", // Invalid
            Type = "InvalidType" // Invalid
        };

        // Act
        var isValid = ArticleMapper.ValidateForMapping(viewModel, out var errors);

        // Assert
        isValid.Should().BeFalse();
        errors.Should().Contain("SKU es requerido");
        errors.Should().Contain("Nombre es requerido");
        errors.Should().Contain("Marca es requerido");
        errors.Should().Contain("Tipo de artículo inválido");
    }

    [Fact]
    public void ValidateForMapping_UpdateViewModel_ValidData_ShouldReturnTrue()
    {
        // Arrange
        var viewModel = new UpdateArticleViewModel
        {
            Id = 1,
            SKU = "ABC123",
            Name = "Test Product",
            Brand = "Test Brand",
            Type = "Footwear"
        };

        // Act
        var isValid = ArticleMapper.ValidateForMapping(viewModel, out var errors);

        // Assert
        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateForMapping_UpdateViewModel_InvalidId_ShouldReturnFalse()
    {
        // Arrange
        var viewModel = new UpdateArticleViewModel
        {
            Id = 0, // Invalid
            SKU = "ABC123",
            Name = "Test Product",
            Brand = "Test Brand",
            Type = "Footwear"
        };

        // Act
        var isValid = ArticleMapper.ValidateForMapping(viewModel, out var errors);

        // Assert
        isValid.Should().BeFalse();
        errors.Should().Contain("ID de artículo debe ser mayor a 0");
    }
}
