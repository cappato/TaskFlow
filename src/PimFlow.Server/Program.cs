using Microsoft.EntityFrameworkCore;
using PimFlow.Server.Data;
using PimFlow.Server.Repositories;
using PimFlow.Server.Services;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Configuration;

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

// Add services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomAttributeService, CustomAttributeService>();

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
