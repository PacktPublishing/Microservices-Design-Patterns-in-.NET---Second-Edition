using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HealthcareDashboard.UI;
using HealthcareDashboard.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var config = builder.Configuration.GetSection("ApiEndpoints");

// Register HttpClient for each service
builder.Services.AddHttpClient<AggregatorServiceClient>(client => client.BaseAddress = new Uri(config["ApiEndpoints:AggregatorServiceApi"]));
// Other service client registrations

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

await builder.Build().RunAsync();
