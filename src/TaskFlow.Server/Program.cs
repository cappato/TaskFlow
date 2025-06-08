using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Server.Repositories;
using TaskFlow.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Railway deployment
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Add services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Add CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Production CORS - allow Railway domains
            policy.WithOrigins("https://*.railway.app", "https://*.up.railway.app")
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

app.UseHttpsRedirection();

app.UseCors("BlazorClient");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint for Railway
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
