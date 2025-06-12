using Microsoft.EntityFrameworkCore;
using PimFlow.Server.Data;
using PimFlow.Server.Configuration;
using PimFlow.Server.Mapping;
using PimFlow.Server.Repositories;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Services;
using PimFlow.Server.Validation;
using PimFlow.Server.Events;
using PimFlow.Domain.Common;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load basic configuration
var featureSettings = builder.Configuration.GetSection(FeatureSettings.SectionName).Get<FeatureSettings>() ?? new FeatureSettings();

// Add basic services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization for enums
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original property names
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PimFlow API", Version = "v1" });

    // Configure custom schema IDs to avoid conflicts
    c.CustomSchemaIds(type =>
    {
        // Handle specific enum conflicts
        if (type.FullName == "PimFlow.Domain.Article.Enums.ArticleType")
            return "DomainArticleType";
        if (type.FullName == "PimFlow.Shared.Enums.ArticleType")
            return "SharedArticleType";
        if (type.FullName == "PimFlow.Domain.CustomAttribute.Enums.AttributeType")
            return "DomainAttributeType";
        if (type.FullName == "PimFlow.Shared.Enums.AttributeType")
            return "SharedAttributeType";

        // Handle generic types (like ApiResponse<T>)
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name.Split('`')[0];
            var genericArgs = type.GetGenericArguments()
                .Select(arg =>
                {
                    if (arg.IsGenericType)
                    {
                        var nestedName = arg.Name.Split('`')[0];
                        var nestedArgs = string.Join("", arg.GetGenericArguments().Select(t => t.Name));
                        return $"{nestedName}Of{nestedArgs}";
                    }
                    return arg.Name;
                })
                .ToArray();
            return $"{genericTypeName}Of{string.Join("And", genericArgs)}";
        }

        // For other types, use the simple name
        return type.Name;
    });

    // Configure enum serialization as strings
    c.UseAllOfToExtendReferenceSchemas();
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ArticleMappingProfile));

// Add basic database (SQLite)
builder.Services.AddDbContext<PimFlowDbContext>(options =>
{
    options.UseSqlite("Data Source=App_Data/application-dev.db");
});

// Add repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICustomAttributeRepository, CustomAttributeRepository>();
builder.Services.AddScoped<IArticleAttributeValueRepository, ArticleAttributeValueRepository>();

// Add query services (solo los que existen)
builder.Services.AddScoped<IArticleQueryService, ArticleQueryService>();
builder.Services.AddScoped<ICustomAttributeQueryService, CustomAttributeQueryService>();

// Add command services (solo los que existen)
builder.Services.AddScoped<IArticleCommandService, ArticleCommandService>();
builder.Services.AddScoped<ICustomAttributeCommandService, CustomAttributeCommandService>();

// Add validation pipeline
builder.Services.AddScoped(typeof(IValidationPipeline<>), typeof(ValidationPipeline<>));

// Add validation services
builder.Services.AddScoped<IArticleValidationService, ArticleValidationService>();

// Add domain event dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Add domain event service
builder.Services.AddScoped<IDomainEventService, DomainEventService>();

// Add basic services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomAttributeService, CustomAttributeService>();

// Add database initialization service
builder.Services.AddScoped<DatabaseInitializationService>();

// Add CORS for development (allow client on different port)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.WithOrigins("http://localhost:5002", "https://localhost:5002")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize database with migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var initService = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    await initService.InitializeAsync();
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PimFlow API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();

    // Enable CORS for development
    app.UseCors("DevelopmentCors");
}

// Configure static files and Blazor WebAssembly
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

// Simple health check
app.MapGet("/health", () => Results.Ok(new {
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

// Fallback to serve the Blazor WebAssembly client
app.MapFallbackToFile("index.html");

app.Run();
