using Microsoft.EntityFrameworkCore;
using PimFlow.Domain.Article;
using PimFlow.Domain.Category;
using PimFlow.Domain.User;
using PimFlow.Domain.CustomAttribute;
using PimFlow.Domain.Article.Enums;
using PimFlow.Domain.CustomAttribute.Enums;
using PimFlow.Server.Data;


namespace PimFlow.Server.Services;

public class DatabaseInitializationService
{
    private readonly PimFlowDbContext _context;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(PimFlowDbContext context, ILogger<DatabaseInitializationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Aplicar migraciones pendientes
            await _context.Database.MigrateAsync();

            // Verificar si ya hay datos de ejemplo
            if (await _context.Articles.AnyAsync())
            {
                _logger.LogInformation("Database already contains sample data. Skipping initialization.");
                return;
            }

            _logger.LogInformation("Initializing database with sample data...");

            // Crear artículos de ejemplo
            await SeedSampleArticlesAsync();

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedSampleArticlesAsync()
    {
        var articles = new[]
        {
            // Nike Products
            new Article { SKU = "NIKE-001", Name = "Nike Air Max 270", Description = "Zapatillas deportivas con tecnología Air Max", Type = ArticleType.Footwear, Brand = "Nike", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "NIKE-002", Name = "Nike Air Force 1 Low", Description = "Zapatillas urbanas con diseño clásico de baloncesto", Type = ArticleType.Footwear, Brand = "Nike", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            
            // Adidas Products
            new Article { SKU = "ADIDAS-001", Name = "Adidas Ultraboost 22", Description = "Zapatillas de running con tecnología Boost", Type = ArticleType.Footwear, Brand = "Adidas", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "ADIDAS-002", Name = "Adidas Stan Smith", Description = "Zapatillas de tenis clásicas con diseño minimalista", Type = ArticleType.Footwear, Brand = "Adidas", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            
            // Puma Products
            new Article { SKU = "PUMA-001", Name = "Puma RS-X3", Description = "Zapatillas retro con diseño futurista", Type = ArticleType.Footwear, Brand = "Puma", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "PUMA-002", Name = "Puma Suede Classic", Description = "Zapatillas de gamuza con estilo retro urbano", Type = ArticleType.Footwear, Brand = "Puma", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            
            // Other Brands
            new Article { SKU = "REEBOK-001", Name = "Reebok Classic Leather", Description = "Zapatillas clásicas de cuero con diseño retro", Type = ArticleType.Footwear, Brand = "Reebok", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "UA-001", Name = "Under Armour HOVR Phantom", Description = "Zapatillas de running con tecnología HOVR", Type = ArticleType.Footwear, Brand = "Under Armour", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "NB-001", Name = "New Balance 990v5", Description = "Zapatillas premium con tecnología ENCAP", Type = ArticleType.Footwear, Brand = "New Balance", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "VANS-001", Name = "Vans Old Skool", Description = "Zapatillas de skate clásicas con diseño icónico", Type = ArticleType.Footwear, Brand = "Vans", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "CONVERSE-001", Name = "Converse Chuck Taylor All Star", Description = "Zapatillas clásicas de lona con diseño atemporal", Type = ArticleType.Footwear, Brand = "Converse", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "ASICS-001", Name = "Asics Gel-Kayano 29", Description = "Zapatillas de running con tecnología GEL para estabilidad", Type = ArticleType.Footwear, Brand = "Asics", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Article { SKU = "SKECHERS-001", Name = "Skechers D'Lites", Description = "Zapatillas chunky con diseño retro y máxima comodidad", Type = ArticleType.Footwear, Brand = "Skechers", CategoryId = 1, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _context.Articles.AddRange(articles);
        await _context.SaveChangesAsync();

        // Obtener los IDs de los artículos creados
        var createdArticles = await _context.Articles
            .Where(a => articles.Select(x => x.SKU).Contains(a.SKU))
            .ToListAsync();

        // Obtener los atributos personalizados existentes
        var attributes = await _context.CustomAttributes
            .Where(a => a.IsActive)
            .ToListAsync();

        // Crear valores de atributos para cada artículo
        await SeedArticleAttributeValuesAsync(createdArticles, attributes);
    }

    private async Task SeedArticleAttributeValuesAsync(List<Article> articles, List<CustomAttribute> attributes)
    {
        var attributeValues = new List<ArticleAttributeValue>();

        // Mapeo de valores por SKU y nombre de atributo
        var articleAttributeData = new Dictionary<string, Dictionary<string, string>>
        {
            ["NIKE-001"] = new() { ["color"] = "Azul/Blanco", ["material"] = "Mesh sintético", ["temporada"] = "Primavera/Verano", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma" },
            ["NIKE-002"] = new() { ["color"] = "Rosa/Blanco", ["material"] = "Cuero sintético", ["temporada"] = "Primavera/Verano", ["genero"] = "Femenino", ["resistencia_agua"] = "false", ["tipo_suela"] = "Air" },
            ["ADIDAS-001"] = new() { ["color"] = "Negro/Rojo", ["material"] = "Primeknit", ["temporada"] = "Otoño/Invierno", ["genero"] = "Masculino", ["resistencia_agua"] = "true", ["tipo_suela"] = "Continental" },
            ["ADIDAS-002"] = new() { ["color"] = "Verde/Blanco", ["material"] = "Cuero genuino", ["temporada"] = "Todo el año", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma" },
            ["PUMA-001"] = new() { ["color"] = "Blanco/Azul", ["material"] = "Cuero sintético", ["temporada"] = "Primavera/Verano", ["genero"] = "Femenino", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma EVA" },
            ["PUMA-002"] = new() { ["color"] = "Azul/Blanco", ["material"] = "Gamuza", ["temporada"] = "Otoño/Invierno", ["genero"] = "Masculino", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma clásica" },
            ["REEBOK-001"] = new() { ["color"] = "Blanco", ["material"] = "Cuero genuino", ["temporada"] = "Todo el año", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma clásica" },
            ["UA-001"] = new() { ["color"] = "Negro/Rojo", ["material"] = "Knit engineered", ["temporada"] = "Otoño/Invierno", ["genero"] = "Masculino", ["resistencia_agua"] = "true", ["tipo_suela"] = "HOVR foam" },
            ["NB-001"] = new() { ["color"] = "Gris", ["material"] = "Mesh y gamuza", ["temporada"] = "Todo el año", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "ENCAP" },
            ["VANS-001"] = new() { ["color"] = "Negro/Blanco", ["material"] = "Canvas y gamuza", ["temporada"] = "Todo el año", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "Waffle" },
            ["CONVERSE-001"] = new() { ["color"] = "Rojo", ["material"] = "Canvas", ["temporada"] = "Primavera/Verano", ["genero"] = "Unisex", ["resistencia_agua"] = "false", ["tipo_suela"] = "Goma vulcanizada" },
            ["ASICS-001"] = new() { ["color"] = "Amarillo/Negro", ["material"] = "Mesh técnico", ["temporada"] = "Primavera/Verano", ["genero"] = "Femenino", ["resistencia_agua"] = "true", ["tipo_suela"] = "GEL" },
            ["SKECHERS-001"] = new() { ["color"] = "Blanco/Rosa", ["material"] = "Cuero sintético", ["temporada"] = "Todo el año", ["genero"] = "Femenino", ["resistencia_agua"] = "false", ["tipo_suela"] = "Memory foam" }
        };

        foreach (var article in articles)
        {
            if (articleAttributeData.TryGetValue(article.SKU, out var attributeData))
            {
                foreach (var attribute in attributes)
                {
                    if (attributeData.TryGetValue(attribute.Name, out var value))
                    {
                        attributeValues.Add(new ArticleAttributeValue
                        {
                            ArticleId = article.Id,
                            CustomAttributeId = attribute.Id,
                            Value = value,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        if (attributeValues.Any())
        {
            _context.ArticleAttributeValues.AddRange(attributeValues);
            await _context.SaveChangesAsync();
        }
    }
}
