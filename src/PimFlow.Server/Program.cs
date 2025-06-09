using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Server.Repositories;
using TaskFlow.Server.Services;
using TaskFlow.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

// Add Entity Framework
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICustomAttributeRepository, CustomAttributeRepository>();
builder.Services.AddScoped<IArticleAttributeValueRepository, ArticleAttributeValueRepository>();

// Add services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomAttributeService, CustomAttributeService>();

// Add CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("https://localhost:7002", "http://localhost:5002", "https://localhost:7001", "http://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Production CORS - allow Azure domains
            policy.WithOrigins("https://*.azurewebsites.net", "https://*.azurestaticapps.net")
                  .SetIsOriginAllowedToAllowWildcardSubdomains()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for development

app.UseCors("BlazorClient");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint for Azure App Service
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
    context.Database.Migrate();
}

app.Run();
