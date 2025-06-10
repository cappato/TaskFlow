using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PimFlow.Server.Data;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;
using PimFlow.Server.Mapping;
using PimFlow.Server.Repositories;
using PimFlow.Server.Services;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Integration;

public class PIMIntegrationTests : IDisposable
{
    private readonly PimFlowDbContext _context;
    private readonly ArticleRepository _articleRepository;
    private readonly CustomAttributeRepository _attributeRepository;
    private readonly ArticleAttributeValueRepository _attributeValueRepository;
    private readonly IMapper _mapper;
    private readonly ArticleService _articleService;
    private readonly CustomAttributeService _attributeService;

    public PIMIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<PimFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PimFlowDbContext(options);
        _articleRepository = new ArticleRepository(_context);
        _attributeRepository = new CustomAttributeRepository(_context);
        _attributeValueRepository = new ArticleAttributeValueRepository(_context);
        var _categoryRepository = new CategoryRepository(_context);

        // Configure AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        // Create CQRS services for integration testing with Strategy Pattern
        var createPipeline = new ValidationPipeline<CreateArticleDto>();
        var updatePipeline = new ValidationPipeline<(int Id, UpdateArticleDto Dto)>();

        // Register validation strategies
        createPipeline.RegisterStrategy(new BasicFieldValidationStrategy());
        createPipeline.RegisterStrategy(new BusinessRulesValidationStrategy(_articleRepository, _categoryRepository));

        updatePipeline.RegisterStrategy(new BasicFieldUpdateValidationStrategy());
        updatePipeline.RegisterStrategy(new BusinessRulesUpdateValidationStrategy(_articleRepository, _categoryRepository));

        var validationService = new ArticleValidationService(createPipeline, updatePipeline);
        var queryService = new ArticleQueryService(_articleRepository, _mapper);
        var commandService = new ArticleCommandService(_articleRepository, _attributeValueRepository, validationService, _mapper);

        _articleService = new ArticleService(queryService, commandService);

        var attributeQueryService = new CustomAttributeQueryService(_attributeRepository);
        var attributeCommandService = new CustomAttributeCommandService(_attributeRepository);
        _attributeService = new CustomAttributeService(attributeQueryService, attributeCommandService);

        // Seed basic data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category
        {
            Id = 1,
            Name = "Calzado Deportivo",
            Description = "Calzado para deportes",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var supplier = new User
        {
            Id = 1,
            Name = "Nike Supplier",
            Email = "supplier@nike.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Categories.Add(category);
        _context.Users.Add(supplier);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CompleteWorkflow_CreateAttributesAndArticles_ShouldWorkEndToEnd()
    {
        // Step 1: Create custom attributes
        var colorAttributeDto = new CreateCustomAttributeDto
        {
            Name = "color",
            DisplayName = "Color",
            Type = DomainEnumMapper.ToShared(AttributeType.Text),
            IsRequired = true,
            SortOrder = 1
        };

        var sizeAttributeDto = new CreateCustomAttributeDto
        {
            Name = "size",
            DisplayName = "Talle",
            Type = DomainEnumMapper.ToShared(AttributeType.Text),
            IsRequired = true,
            SortOrder = 2
        };

        var waterproofAttributeDto = new CreateCustomAttributeDto
        {
            Name = "waterproof",
            DisplayName = "Resistente al Agua",
            Type = DomainEnumMapper.ToShared(AttributeType.Boolean),
            IsRequired = false,
            SortOrder = 3
        };

        // Create attributes
        var colorAttribute = await _attributeService.CreateAttributeAsync(colorAttributeDto);
        var sizeAttribute = await _attributeService.CreateAttributeAsync(sizeAttributeDto);
        var waterproofAttribute = await _attributeService.CreateAttributeAsync(waterproofAttributeDto);

        // Verify attributes were created
        colorAttribute.Should().NotBeNull();
        colorAttribute.Name.Should().Be("color");
        sizeAttribute.Name.Should().Be("size");
        waterproofAttribute.Name.Should().Be("waterproof");

        // Step 2: Create article with custom attributes
        var articleDto = new CreateArticleDto
        {
            SKU = "NIKE-AIR-001",
            Name = "Nike Air Max 90",
            Description = "Zapatillas deportivas clásicas",
            Type = DomainEnumMapper.ToShared(ArticleType.Footwear),
            Brand = "Nike",
            CategoryId = 1,
            SupplierId = 1,
            CustomAttributes = new Dictionary<string, object>
            {
                { "color", "Blanco/Negro" },
                { "size", "42" },
                { "waterproof", "true" }
            }
        };

        var createdArticle = await _articleService.CreateArticleAsync(articleDto);

        // Verify article was created with attributes
        createdArticle.Should().NotBeNull();
        createdArticle.SKU.Should().Be("NIKE-AIR-001");
        createdArticle.Name.Should().Be("Nike Air Max 90");
        createdArticle.CustomAttributes.Should().HaveCount(3);
        createdArticle.CustomAttributes["color"].Should().Be("Blanco/Negro");
        createdArticle.CustomAttributes["size"].Should().Be("42");
        createdArticle.CustomAttributes["waterproof"].Should().Be("true");

        // Step 3: Search by custom attribute
        var redShoes = await _articleService.GetArticlesByAttributeAsync("color", "Blanco");
        redShoes.Should().HaveCount(1);
        redShoes.First().SKU.Should().Be("NIKE-AIR-001");

        // Step 4: Create another article with different attributes
        var adidasDto = new CreateArticleDto
        {
            SKU = "ADIDAS-UB-001",
            Name = "Adidas Ultraboost 22",
            Description = "Zapatillas de running",
            Type = DomainEnumMapper.ToShared(ArticleType.Footwear),
            Brand = "Adidas",
            CategoryId = 1,
            SupplierId = 1,
            CustomAttributes = new Dictionary<string, object>
            {
                { "color", "Azul" },
                { "size", "41" },
                { "waterproof", "false" }
            }
        };

        var adidasArticle = await _articleService.CreateArticleAsync(adidasDto);

        // Step 5: Verify we can search and filter by attributes
        var allArticles = await _articleService.GetAllArticlesAsync();
        allArticles.Should().HaveCount(2);

        var blueShoes = await _articleService.GetArticlesByAttributeAsync("color", "Azul");
        blueShoes.Should().HaveCount(1);
        blueShoes.First().Brand.Should().Be("Adidas");

        var size42Shoes = await _articleService.GetArticlesByAttributeAsync("size", "42");
        size42Shoes.Should().HaveCount(1);
        size42Shoes.First().Brand.Should().Be("Nike");

        // Step 6: Update article attributes
        var updateDto = new UpdateArticleDto
        {
            CustomAttributes = new Dictionary<string, object>
            {
                { "color", "Blanco/Azul" },
                { "size", "43" },
                { "waterproof", "true" }
            }
        };

        var updatedArticle = await _articleService.UpdateArticleAsync(createdArticle.Id, updateDto);
        updatedArticle.Should().NotBeNull();
        updatedArticle!.CustomAttributes["color"].Should().Be("Blanco/Azul");
        updatedArticle.CustomAttributes["size"].Should().Be("43");

        // Step 7: Verify search still works after update
        var updatedBlueShoes = await _articleService.GetArticlesByAttributeAsync("color", "Azul");
        updatedBlueShoes.Should().HaveCount(2); // Both articles contain "Azul"
        updatedBlueShoes.Should().Contain(a => a.SKU == "NIKE-AIR-001");
        updatedBlueShoes.Should().Contain(a => a.SKU == "ADIDAS-UB-001");
    }

    [Fact]
    public async Task AttributeManagement_CreateUpdateDelete_ShouldWorkCorrectly()
    {
        // Create attribute
        var createDto = new CreateCustomAttributeDto
        {
            Name = "material",
            DisplayName = "Material",
            Type = DomainEnumMapper.ToShared(AttributeType.Text),
            IsRequired = false,
            SortOrder = 1
        };

        var created = await _attributeService.CreateAttributeAsync(createDto);
        created.Should().NotBeNull();
        created.Name.Should().Be("material");

        // Update attribute
        var updateDto = new UpdateCustomAttributeDto
        {
            DisplayName = "Material del Producto",
            IsRequired = true,
            Type = DomainEnumMapper.ToShared(AttributeType.Select)
        };

        var updated = await _attributeService.UpdateAttributeAsync(created.Id, updateDto);
        updated.Should().NotBeNull();
        updated!.DisplayName.Should().Be("Material del Producto");
        updated.IsRequired.Should().BeTrue();
        updated.Type.Should().Be(DomainEnumMapper.ToShared(AttributeType.Select));

        // Verify it appears in active attributes
        var activeAttributes = await _attributeService.GetActiveAttributesAsync();
        activeAttributes.Should().Contain(x => x.Name == "material");

        // Delete attribute
        var deleted = await _attributeService.DeleteAttributeAsync(created.Id);
        deleted.Should().BeTrue();

        // Verify it's gone
        var afterDelete = await _attributeService.GetAttributeByIdAsync(created.Id);
        afterDelete.Should().BeNull();
    }

    [Fact]
    public async Task DuplicateValidation_ShouldPreventDuplicates()
    {
        // Create first attribute
        var firstDto = new CreateCustomAttributeDto
        {
            Name = "unique_name",
            DisplayName = "Unique Attribute"
        };

        await _attributeService.CreateAttributeAsync(firstDto);

        // Try to create duplicate
        var duplicateDto = new CreateCustomAttributeDto
        {
            Name = "unique_name",
            DisplayName = "Duplicate Attribute"
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _attributeService.CreateAttributeAsync(duplicateDto));

        exception.Message.Should().Contain("Ya existe un atributo con nombre: unique_name");

        // Same for articles
        var firstArticleDto = new CreateArticleDto
        {
            SKU = "UNIQUE-SKU-001",
            Name = "First Article",
            CategoryId = 1 // Required by new validation
        };

        await _articleService.CreateArticleAsync(firstArticleDto);

        var duplicateArticleDto = new CreateArticleDto
        {
            SKU = "UNIQUE-SKU-001",
            Name = "Duplicate Article",
            CategoryId = 1 // Required by new validation
        };

        var articleException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _articleService.CreateArticleAsync(duplicateArticleDto));

        articleException.Message.Should().Contain("Ya existe un artículo con SKU: UNIQUE-SKU-001");
    }

    [Fact]
    public async Task SearchFunctionality_ShouldWorkAcrossAllFields()
    {
        // Create test article
        var articleDto = new CreateArticleDto
        {
            SKU = "SEARCH-TEST-001",
            Name = "Zapatillas de Running",
            Description = "Perfectas para correr en asfalto",
            Brand = "TestBrand",
            Type = DomainEnumMapper.ToShared(ArticleType.Footwear),
            CategoryId = 1 // Required by new validation
        };

        await _articleService.CreateArticleAsync(articleDto);

        // Search by name
        var nameResults = await _articleService.SearchArticlesAsync("Running");
        nameResults.Should().HaveCount(1);

        // Search by SKU
        var skuResults = await _articleService.SearchArticlesAsync("SEARCH-TEST");
        skuResults.Should().HaveCount(1);

        // Search by brand
        var brandResults = await _articleService.SearchArticlesAsync("TestBrand");
        brandResults.Should().HaveCount(1);

        // Search by description
        var descResults = await _articleService.SearchArticlesAsync("asfalto");
        descResults.Should().HaveCount(1);

        // Search with no results
        var noResults = await _articleService.SearchArticlesAsync("NoExiste");
        noResults.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
