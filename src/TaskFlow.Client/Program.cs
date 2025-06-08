using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskFlow.Client;
using TaskFlow.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7000/")
});

// Add services
builder.Services.AddScoped<ITaskApiService, TaskApiService>();
builder.Services.AddScoped<IProjectApiService, ProjectApiService>();

await builder.Build().RunAsync();
