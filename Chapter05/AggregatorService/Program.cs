using AggregatorService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register AppointmentsServiceClient
builder.Services.AddHttpClient<AppointmentsServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:AppointmentsApi"]);
});

builder.Services.AddHttpClient<PatientServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:PatientApi"]);
});
builder.Services.AddHttpClient<MedicalHistoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:MedicalHistoryApi"]);
});
builder.Services.AddHttpClient<BillingServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:BillingApi"]);
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();