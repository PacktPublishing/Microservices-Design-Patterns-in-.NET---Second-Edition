using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Services;
using MassTransit;
using Notification.Service;

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

// Other service configurations
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AppointmentCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        // Configure the receive endpoint
        cfg.ReceiveEndpoint("appointment_created_queue", e =>
        {
            e.PrefetchCount = 1; // Fetch one message at a time
            e.UseConcurrencyLimit(1); // Process one message at a time
            e.ConfigureConsumer<AppointmentCreatedConsumer>(context);
        });
    });
});
// Other service configurations

builder.Services.AddTransient<IEmailService, EmailService>();

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
