using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PimFlow.Client;
using PimFlow.Client.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls with proper JSON options
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5001/")
    };
    return httpClient;
});

// Configure JSON serialization options as a service
builder.Services.AddSingleton<JsonSerializerOptions>(provider =>
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null, // Keep original property names
    };
    options.Converters.Add(new JsonStringEnumConverter());
    return options;
});

// Add services
builder.Services.AddScoped<IArticleApiService, ArticleApiService>();
builder.Services.AddScoped<ICustomAttributeApiService, CustomAttributeApiService>();

await builder.Build().RunAsync();
