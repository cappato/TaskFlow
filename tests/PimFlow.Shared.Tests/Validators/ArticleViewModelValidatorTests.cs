using FluentAssertions;
using FluentValidation.Results;
using PimFlow.Shared.Common;
using PimFlow.Shared.Validators;
using PimFlow.Shared.ViewModels;
using Xunit;

namespace PimFlow.Shared.Tests.Validators;

public class ArticleViewModelValidatorTests
{
    private readonly CreateArticleViewModelValidator _createValidator = new();
    private readonly UpdateArticleViewModelValidator _updateValidator = new();

    private CreateArticleViewModel CreateValidCreateViewModel()
    {
        return new CreateArticleViewModel
        {
            SKU = "ABC123",
            Name = "Test Product",
            Description = "Test Description",
            Type = "Footwear",
            Brand = "Test Brand",
            CategoryId = 1,
            SupplierId = 1,
            CustomAttributes = new Dictionary<string, object>()
        };
    }

    private UpdateArticleViewModel CreateValidUpdateViewModel()
    {
        return new UpdateArticleViewModel
        {
            Id = 1,
            SKU = "ABC123",
            Name = "Test Product",
            Description = "Test Description",
            Type = "Footwear",
            Brand = "Test Brand",
            CategoryId = 1,
            SupplierId = 1,
            CustomAttributes = new Dictionary<string, object>()
        };
    }

    #region CreateArticleViewModel Tests

    [Fact]
    public async Task CreateValidator_ValidModel_ShouldPass()
    {
        // Arrange
        var model = CreateValidCreateViewModel();

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateValidator_EmptySKU_ShouldFail(string sku)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = sku;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "SKU es requerido");
    }

    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("A")] // Too short
    public async Task CreateValidator_TooShortSKU_ShouldFail(string sku)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = sku;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "SKU debe tener entre 3 y 50 caracteres");
    }

    [Fact]
    public async Task CreateValidator_TooLongSKU_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = new string('A', 51); // 51 characters

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "SKU debe tener entre 3 y 50 caracteres");
    }

    [Theory]
    [InlineData("ABC@123")] // Contains special character
    [InlineData("ABC 123")] // Contains space
    [InlineData("abc123")] // Contains lowercase
    public async Task CreateValidator_InvalidSKUFormat_ShouldFail(string sku)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = sku;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "SKU solo puede contener letras mayúsculas, números y guiones");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateValidator_EmptyName_ShouldFail(string name)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Name = name;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Nombre es requerido");
    }

    [Theory]
    [InlineData("A")] // Too short
    public async Task CreateValidator_TooShortName_ShouldFail(string name)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Name = name;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Nombre debe tener entre 2 y 200 caracteres");
    }

    [Fact]
    public async Task CreateValidator_TooLongName_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Name = new string('A', 201); // 201 characters

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Nombre debe tener entre 2 y 200 caracteres");
    }

    [Fact]
    public async Task CreateValidator_TooLongDescription_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Description = new string('A', 1001); // 1001 characters

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Descripción no puede exceder 1000 caracteres");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateValidator_EmptyType_ShouldFail(string type)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Type = type;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Tipo es requerido");
    }

    [Fact]
    public async Task CreateValidator_InvalidType_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Type = "InvalidType";

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Tipo de artículo inválido");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateValidator_EmptyBrand_ShouldFail(string brand)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Brand = brand;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Marca es requerida");
    }

    [Theory]
    [InlineData("A")] // Too short
    public async Task CreateValidator_TooShortBrand_ShouldFail(string brand)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Brand = brand;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Marca debe tener entre 2 y 100 caracteres");
    }

    [Fact]
    public async Task CreateValidator_TooLongBrand_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.Brand = new string('A', 101); // 101 characters

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Marca debe tener entre 2 y 100 caracteres");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateValidator_InvalidCategoryId_ShouldFail(int categoryId)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.CategoryId = categoryId;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "ID de categoría debe ser mayor a 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateValidator_InvalidSupplierId_ShouldFail(int supplierId)
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SupplierId = supplierId;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "ID de proveedor debe ser mayor a 0");
    }

    [Fact]
    public async Task CreateValidator_NullCustomAttributes_ShouldFail()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.CustomAttributes = null!;

        // Act
        var result = await _createValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Atributos personalizados no pueden ser nulos");
    }

    #endregion

    #region UpdateArticleViewModel Tests

    [Fact]
    public async Task UpdateValidator_ValidModel_ShouldPass()
    {
        // Arrange
        var model = CreateValidUpdateViewModel();

        // Act
        var result = await _updateValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateValidator_InvalidId_ShouldFail(int id)
    {
        // Arrange
        var model = CreateValidUpdateViewModel();
        model.Id = id;

        // Act
        var result = await _updateValidator.ValidateAsync(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "ID de artículo debe ser mayor a 0");
    }

    // Note: Other validation rules are the same as CreateValidator, so we don't need to repeat all tests

    #endregion

    #region Extension Methods Tests

    [Fact]
    public async Task ValidateAsync_CreateValidator_ValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = CreateValidCreateViewModel();

        // Act
        var response = await _createValidator.ValidateAsync(model);

        // Assert
        response.IsValid.Should().BeTrue();
        response.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_CreateValidator_InvalidModel_ShouldReturnValidationError()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = ""; // Invalid

        // Act
        var response = await _createValidator.ValidateAsync(model);

        // Assert
        response.IsValid.Should().BeFalse();
        response.Errors.Should().Contain(e => e.ErrorMessage == "SKU es requerido");
    }

    [Fact]
    public async Task ValidatePropertyAsync_SpecificProperty_ShouldValidateOnlyThatProperty()
    {
        // Arrange
        var model = CreateValidCreateViewModel();
        model.SKU = ""; // Invalid
        model.Name = ""; // Also invalid, but we're only validating SKU

        // Act
        ApiResponse result = await _createValidator.ValidatePropertyAsync(model, nameof(model.SKU));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().Contain("SKU es requerido");
        result.ValidationErrors.Should().NotContain(e => e.Contains("Nombre")); // Should not validate Name
    }

    #endregion
}
