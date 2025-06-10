using AutoMapper;
using FluentAssertions;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using PimFlow.Server.Mapping;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Mapping;

/// <summary>
/// Tests de integración para AutoMapper que validan escenarios complejos
/// Estos tests aseguran que AutoMapper funciona correctamente en casos reales
/// </summary>
public class AutoMapperIntegrationTests
{
    private readonly IMapper _mapper;

    public AutoMapperIntegrationTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ArticleWithComplexRelations_ShouldMapCorrectly()
    {
        // Arrange - Article con todas las relaciones
        var article = new Article
        {
            Id = 1,
            SKU = "COMPLEX-001",
            Name = "Complex Article",
            Description = "Article with all relations",
            Type = ArticleType.Footwear,
            Brand = "Test Brand",
            CategoryId = 10,
            SupplierId = 20,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2),
            IsActive = true,
            Category = new Category 
            { 
                Id = 10, 
                Name = "Test Category",
                Articles = new List<Article>() // Evitar referencia circular
            },
            Supplier = new User 
            { 
                Id = 20, 
                Name = "Test Supplier",
                Email = "supplier@test.com"
            },
            AttributeValues = new List<ArticleAttributeValue>
            {
                new ArticleAttributeValue
                {
                    Id = 1,
                    ArticleId = 1,
                    CustomAttributeId = 100,
                    Value = "Red",
                    CustomAttribute = new CustomAttribute
                    {
                        Id = 100,
                        Name = "Color",
                        DisplayName = "Color",
                        Type = AttributeType.Text
                    }
                },
                new ArticleAttributeValue
                {
                    Id = 2,
                    ArticleId = 1,
                    CustomAttributeId = 101,
                    Value = "42",
                    CustomAttribute = new CustomAttribute
                    {
                        Id = 101,
                        Name = "Size",
                        DisplayName = "Size",
                        Type = AttributeType.Number
                    }
                }
            },
            Variants = new List<ArticleVariant>
            {
                new ArticleVariant
                {
                    Id = 1,
                    SKU = "COMPLEX-001-RED-42",
                    ArticleId = 1,
                    Size = "42",
                    Color = "Red",
                    Stock = 10,
                    WholesalePrice = 50.00m,
                    RetailPrice = 80.00m,
                    IsActive = true
                }
            }
        };

        // Act
        var dto = _mapper.Map<ArticleDto>(article);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.SKU.Should().Be("COMPLEX-001");
        dto.Name.Should().Be("Complex Article");
        dto.CategoryName.Should().Be("Test Category");
        dto.SupplierName.Should().Be("Test Supplier");
        
        // Verificar custom attributes
        dto.CustomAttributes.Should().NotBeNull();
        dto.CustomAttributes.Should().HaveCount(2);
        dto.CustomAttributes.Should().ContainKey("Color");
        dto.CustomAttributes.Should().ContainKey("Size");
        dto.CustomAttributes["Color"].Should().Be("Red");
        dto.CustomAttributes["Size"].Should().Be("42");
    }

    // NOTE: Category mapping test moved to AutoMapperTests.cs
    // This test was causing issues because Category mapping is part of ArticleMappingProfile

    [Fact]
    public void CollectionMapping_ShouldHandleLargeCollections()
    {
        // Arrange - Crear una colección grande de artículos
        var articles = Enumerable.Range(1, 500).Select(i => new Article
        {
            Id = i,
            SKU = $"BULK-{i:000}",
            Name = $"Bulk Article {i}",
            Description = $"Description for article {i}",
            Type = i % 2 == 0 ? ArticleType.Footwear : ArticleType.Clothing,
            Brand = $"Brand {i % 10}",
            CategoryId = i % 5 + 1,
            SupplierId = i % 3 + 1,
            CreatedAt = DateTime.UtcNow.AddDays(-i),
            IsActive = i % 10 != 0, // 90% activos
            Category = new Category { Id = i % 5 + 1, Name = $"Category {i % 5 + 1}" },
            Supplier = new User { Id = i % 3 + 1, Name = $"Supplier {i % 3 + 1}" }
        }).ToList();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var dtos = _mapper.Map<List<ArticleDto>>(articles);
        stopwatch.Stop();

        // Assert
        dtos.Should().HaveCount(500);
        dtos.Should().AllSatisfy(dto =>
        {
            dto.Should().NotBeNull();
            dto.Id.Should().BeGreaterThan(0);
            dto.SKU.Should().StartWith("BULK-");
            dto.CategoryName.Should().NotBeNullOrEmpty();
            dto.SupplierName.Should().NotBeNullOrEmpty();
        });
        
        // Performance assertion
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(200, 
            "AutoMapper should handle 500 articles in less than 200ms");
    }

    [Fact]
    public void NullHandling_ShouldHandleNullPropertiesGracefully()
    {
        // Arrange - Article con propiedades null
        var article = new Article
        {
            Id = 1,
            SKU = "NULL-TEST",
            Name = "Null Test Article",
            Description = null!, // null
            Type = ArticleType.Footwear,
            Brand = null!, // null
            CategoryId = null, // null
            SupplierId = null, // null
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Category = null!, // null
            Supplier = null!, // null
            AttributeValues = null!, // null
            Variants = null! // null
        };

        // Act
        var dto = _mapper.Map<ArticleDto>(article);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.SKU.Should().Be("NULL-TEST");
        dto.Name.Should().Be("Null Test Article");
        dto.Description.Should().BeNull();
        dto.Brand.Should().BeNull();
        dto.CategoryId.Should().BeNull();
        dto.CategoryName.Should().BeNull();
        dto.SupplierId.Should().BeNull();
        dto.SupplierName.Should().BeNull();
        dto.CustomAttributes.Should().NotBeNull();
        dto.CustomAttributes.Should().BeEmpty();
    }

    [Fact]
    public void EnumMapping_ShouldMapAllEnumValues()
    {
        // Arrange & Act & Assert - Probar todos los valores de enum
        foreach (ArticleType domainType in Enum.GetValues<ArticleType>())
        {
            var article = new Article
            {
                Id = 1,
                SKU = "ENUM-TEST",
                Name = "Enum Test",
                Type = domainType,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var dto = _mapper.Map<ArticleDto>(article);

            dto.Type.ToString().Should().Be(domainType.ToString(),
                $"Enum mapping should work for {domainType}");
        }
    }
}
