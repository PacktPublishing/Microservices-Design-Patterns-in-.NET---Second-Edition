using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppointmentContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register PatientsApiClient
builder.Services.AddHttpClient<PatientsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:PatientsApi"]);
});

// Register DoctorsApiClient
builder.Services.AddHttpClient<DoctorsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:DoctorsApi"]);
});


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
