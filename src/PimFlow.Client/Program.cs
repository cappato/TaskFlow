using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PimFlow.Client;
using PimFlow.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5001/")
});

// Add services
builder.Services.AddScoped<IArticleApiService, ArticleApiService>();
builder.Services.AddScoped<ICustomAttributeApiService, CustomAttributeApiService>();

await builder.Build().RunAsync();
