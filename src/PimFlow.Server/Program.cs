using Microsoft.EntityFrameworkCore;
using PimFlow.Server.Data;
using PimFlow.Server.Repositories;
using PimFlow.Server.Services;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Configuration;
using PimFlow.Server.Mapping;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;
using PimFlow.Shared.DTOs;
using PimFlow.Domain.Events;
using PimFlow.Server.Events;
using PimFlow.Server.Events.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Load configuration settings
var appSettings = builder.Configuration.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>() ?? new ApplicationSettings();
var dbSettings = builder.Configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>() ?? new DatabaseSettings();
var featureSettings = builder.Configuration.GetSection(FeatureSettings.SectionName).Get<FeatureSettings>() ?? new FeatureSettings();

// Configure for Azure App Service
if (!builder.Environment.IsDevelopment())
{
    // Azure App Service handles port configuration automatically
    // Ensure data directory exists for SQLite
    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Blazor WebAssembly hosting
builder.Services.AddRazorPages();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ArticleMappingProfile));

// Add Entity Framework with centralized configuration
builder.Services.AddDbContext<PimFlowDbContext>(options =>
{
    var connectionString = dbSettings.ConnectionString ?? builder.Configuration.GetConnectionString("DefaultConnection");

    switch (dbSettings.Provider.ToUpperInvariant())
    {
        case "SQLITE":
            options.UseSqlite(connectionString);
            break;
        case "SQLSERVER":
            options.UseSqlServer(connectionString);
            break;
        default:
            options.UseSqlite(connectionString); // Default fallback
            break;
    }
});

// Add repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICustomAttributeRepository, CustomAttributeRepository>();
builder.Services.AddScoped<IArticleAttributeValueRepository, ArticleAttributeValueRepository>();

// Add services - CQRS pattern implementation
// Query services (read operations)
builder.Services.AddScoped<IArticleQueryService, ArticleQueryService>();
builder.Services.AddScoped<ICustomAttributeQueryService, CustomAttributeQueryService>();

// Command services (write operations)
builder.Services.AddScoped<IArticleCommandService, ArticleCommandService>();
builder.Services.AddScoped<ICustomAttributeCommandService, CustomAttributeCommandService>();

// Validation services - Strategy Pattern (Open/Closed Principle)
builder.Services.AddScoped<IValidationPipeline<CreateArticleDto>, ValidationPipeline<CreateArticleDto>>();
builder.Services.AddScoped<IValidationPipeline<(int Id, UpdateArticleDto Dto)>, ValidationPipeline<(int Id, UpdateArticleDto Dto)>>();

// Validation strategies - NEW STRATEGIES CAN BE ADDED WITHOUT MODIFYING EXISTING CODE
builder.Services.AddScoped<IArticleCreateValidationStrategy, BasicFieldValidationStrategy>();
builder.Services.AddScoped<IArticleCreateValidationStrategy, BusinessRulesValidationStrategy>();
builder.Services.AddScoped<IArticleUpdateValidationStrategy, BasicFieldUpdateValidationStrategy>();
builder.Services.AddScoped<IArticleUpdateValidationStrategy, BusinessRulesUpdateValidationStrategy>();

// Register strategies in pipelines
builder.Services.AddScoped<IArticleValidationService>(provider =>
{
    var createPipeline = provider.GetRequiredService<IValidationPipeline<CreateArticleDto>>();
    var updatePipeline = provider.GetRequiredService<IValidationPipeline<(int Id, UpdateArticleDto Dto)>>();

    // Register create strategies
    var createStrategies = provider.GetServices<IArticleCreateValidationStrategy>();
    foreach (var strategy in createStrategies)
    {
        createPipeline.RegisterStrategy(strategy);
    }

    // Register update strategies
    var updateStrategies = provider.GetServices<IArticleUpdateValidationStrategy>();
    foreach (var strategy in updateStrategies)
    {
        updatePipeline.RegisterStrategy(strategy);
    }

    return new ArticleValidationService(createPipeline, updatePipeline);
});

// Domain Events Infrastructure
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();

// Domain Event Handlers - Article Events
builder.Services.AddScoped<IDomainEventHandler<ArticleCreatedEvent>, ArticleCreatedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ArticleUpdatedEvent>, ArticleUpdatedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ArticleDeletedEvent>, ArticleDeletedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ArticleCategoryChangedEvent>, ArticleCategoryChangedEventHandler>();

// Domain Event Handlers - Category Events
builder.Services.AddScoped<IDomainEventHandler<CategoryCreatedEvent>, CategoryCreatedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<CategoryUpdatedEvent>, CategoryUpdatedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<CategoryDeletedEvent>, CategoryDeletedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<CategoryHierarchyChangedEvent>, CategoryHierarchyChangedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ArticleAddedToCategoryEvent>, ArticleAddedToCategoryEventHandler>();

// Facade services (backward compatibility)
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomAttributeService, CustomAttributeService>();

// INTERFACE SEGREGATION PRINCIPLE (ISP) - Segregated interfaces registration
// Article segregated interfaces
builder.Services.AddScoped<IArticleReader>(provider => provider.GetRequiredService<ArticleService>());
builder.Services.AddScoped<IArticleFilter>(provider => provider.GetRequiredService<ArticleService>());
builder.Services.AddScoped<IArticleWriter>(provider => provider.GetRequiredService<ArticleService>());

// Category segregated interfaces
builder.Services.AddScoped<ICategoryReader>(provider => provider.GetRequiredService<CategoryService>());
builder.Services.AddScoped<ICategoryHierarchy>(provider => provider.GetRequiredService<CategoryService>());
builder.Services.AddScoped<ICategoryWriter>(provider => provider.GetRequiredService<CategoryService>());

// CustomAttribute segregated interfaces
builder.Services.AddScoped<ICustomAttributeReader>(provider => provider.GetRequiredService<CustomAttributeService>());
builder.Services.AddScoped<ICustomAttributeWriter>(provider => provider.GetRequiredService<CustomAttributeService>());

// CORS no longer needed in Hosted architecture - Client served from same origin

var app = builder.Build();

// Configure the HTTP request pipeline based on feature flags
if (featureSettings.EnableSwagger && app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    if (featureSettings.EnableDetailedErrors)
    {
        app.UseDeveloperExceptionPage();
    }
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

// Health check endpoint with application info
app.MapGet("/health", () => Results.Ok(new {
    status = "healthy",
    application = appSettings.Name,
    version = appSettings.Version,
    environment = appSettings.Environment,
    timestamp = DateTime.UtcNow
}));

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PimFlowDbContext>();
    context.Database.Migrate();
}

app.Run();
