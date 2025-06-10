using AutoMapper;
using FluentAssertions;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using PimFlow.Server.Mapping;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Mapping;

/// <summary>
/// Tests para verificar que AutoMapper funciona correctamente
/// Estos tests validan que el mapeo centralizado reduce el acoplamiento
/// </summary>
public class AutoMapperTests
{
    private readonly IMapper _mapper;

    public AutoMapperTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void AutoMapper_Configuration_ShouldBeValid()
    {
        // Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void ArticleToDto_ShouldMapCorrectly()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Test Article",
            Description = "Test Description",
            Type = ArticleType.Footwear,
            Brand = "Test Brand",
            CategoryId = 10,
            SupplierId = 20,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2),
            IsActive = true,
            Category = new Category { Id = 10, Name = "Test Category" },
            Supplier = new User { Id = 20, Name = "Test Supplier" }
        };

        // Act
        var dto = _mapper.Map<ArticleDto>(article);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(article.Id);
        dto.SKU.Should().Be(article.SKU);
        dto.Name.Should().Be(article.Name);
        dto.Description.Should().Be(article.Description);
        dto.Type.Should().Be(Shared.Enums.ArticleType.Footwear);
        dto.Brand.Should().Be(article.Brand);
        dto.CategoryId.Should().Be(article.CategoryId);
        dto.CategoryName.Should().Be("Test Category");
        dto.SupplierId.Should().Be(article.SupplierId);
        dto.SupplierName.Should().Be("Test Supplier");
        dto.CreatedAt.Should().Be(article.CreatedAt);
        dto.UpdatedAt.Should().Be(article.UpdatedAt);
        dto.IsActive.Should().Be(article.IsActive);
    }

    [Fact]
    public void CreateArticleDtoToEntity_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "NEW-001",
            Name = "New Article",
            Description = "New Description",
            Type = Shared.Enums.ArticleType.Clothing,
            Brand = "New Brand",
            CategoryId = 15,
            SupplierId = 25,
            CustomAttributes = new Dictionary<string, object>
            {
                { "Color", "Red" },
                { "Size", "M" }
            }
        };

        // Act
        var entity = _mapper.Map<Article>(createDto);

        // Assert
        entity.Should().NotBeNull();
        entity.SKU.Should().Be(createDto.SKU);
        entity.Name.Should().Be(createDto.Name);
        entity.Description.Should().Be(createDto.Description);
        entity.Type.Should().Be(ArticleType.Clothing);
        entity.Brand.Should().Be(createDto.Brand);
        entity.CategoryId.Should().Be(createDto.CategoryId);
        entity.SupplierId.Should().Be(createDto.SupplierId);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CategoryToDto_ShouldMapCorrectly()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test Description",
            ParentCategoryId = 5,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1),
            ParentCategory = new Category { Id = 5, Name = "Parent Category" },
            SubCategories = new List<Category>
            {
                new Category { Id = 2, Name = "Sub 1" },
                new Category { Id = 3, Name = "Sub 2" }
            }
        };

        // Act
        var dto = _mapper.Map<CategoryDto>(category);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(category.Id);
        dto.Name.Should().Be(category.Name);
        dto.Description.Should().Be(category.Description);
        dto.ParentCategoryId.Should().Be(category.ParentCategoryId);
        dto.ParentCategoryName.Should().Be("Parent Category");
        dto.SubCategoryCount.Should().Be(2);
        dto.IsActive.Should().Be(category.IsActive);
        dto.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact]
    public void CustomAttributeToDto_ShouldMapCorrectly()
    {
        // Arrange
        var attribute = new CustomAttribute
        {
            Id = 1,
            Name = "Color",
            DisplayName = "Color",
            Type = Domain.Enums.AttributeType.Text,
            IsRequired = true,
            DefaultValue = "Blue",
            ValidationRules = "required",
            SortOrder = 1,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var dto = _mapper.Map<CustomAttributeDto>(attribute);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(attribute.Id);
        dto.Name.Should().Be(attribute.Name);
        dto.DisplayName.Should().Be(attribute.DisplayName);
        dto.Type.Should().Be(Shared.Enums.AttributeType.Text);
        dto.IsRequired.Should().Be(attribute.IsRequired);
        dto.DefaultValue.Should().Be(attribute.DefaultValue);
        dto.ValidationRules.Should().Be(attribute.ValidationRules);
        dto.SortOrder.Should().Be(attribute.SortOrder);
        dto.IsActive.Should().Be(attribute.IsActive);
        dto.CreatedAt.Should().Be(attribute.CreatedAt);
    }

    [Fact]
    public void ArticleVariantToDto_ShouldMapCorrectly()
    {
        // Arrange
        var variant = new ArticleVariant
        {
            Id = 1,
            SKU = "VAR-001",
            ArticleId = 10,
            Size = "42",
            Color = "Red",
            Stock = 100,
            WholesalePrice = 50.00m,
            RetailPrice = 80.00m,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2)
        };

        // Act
        var dto = _mapper.Map<ArticleVariantDto>(variant);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(variant.Id);
        dto.SKU.Should().Be(variant.SKU);
        dto.ArticleId.Should().Be(variant.ArticleId);
        dto.Size.Should().Be(variant.Size);
        dto.Color.Should().Be(variant.Color);
        dto.Stock.Should().Be(variant.Stock);
        dto.WholesalePrice.Should().Be(variant.WholesalePrice);
        dto.RetailPrice.Should().Be(variant.RetailPrice);
        dto.IsActive.Should().Be(variant.IsActive);
        dto.CreatedAt.Should().Be(variant.CreatedAt);
        dto.UpdatedAt.Should().Be(variant.UpdatedAt);
    }

    [Fact]
    public void MappingPerformance_ShouldBeAcceptable()
    {
        // Arrange
        var articles = Enumerable.Range(1, 1000).Select(i => new Article
        {
            Id = i,
            SKU = $"TEST-{i:000}",
            Name = $"Test Article {i}",
            Type = ArticleType.Footwear,
            Brand = "Test Brand",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }).ToList();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var dtos = _mapper.Map<List<ArticleDto>>(articles);
        stopwatch.Stop();

        // Assert
        dtos.Should().HaveCount(1000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "AutoMapper should be fast enough for production use");
    }
}
