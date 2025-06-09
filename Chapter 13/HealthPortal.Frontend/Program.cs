using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HealthPortal.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("AppointmentsAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/api/appointments/");
});

builder.Services.AddHttpClient("DoctorsAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:5002/api/doctors/");
});

builder.Services.AddHttpClient("PatientsAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:5003/api/patients/");
});

builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri("https://gateway.example.com/api/");
});

builder.Services.AddSingleton<UserSessionState>();


await builder.Build().RunAsync();
