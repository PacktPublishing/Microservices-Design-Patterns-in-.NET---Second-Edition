using AppointmentsApi.Consumers;
using AppointmentsApi.Data;
using AppointmentsApi.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notification.Service;
using Shared.Data;
using Shared.SagaOrchestrator;
using System.Reflection;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppointmentContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EventStoreDatabase")));

builder.Services.AddDbContext<BookingSagaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SagaDbContext")));


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
    x.AddConsumer<AppointmentCanceledConsumer>();
    x.AddConsumer<AppointmentCreatedConsumer>();
    x.AddSagaStateMachine<BookingStateMachine, BookingSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, BookingSagaDbContext>();
            r.UsePostgres();
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        // Configure the receive endpoint
        cfg.ReceiveEndpoint("appointment_created_queue", e =>
        {
            e.PrefetchCount = 1; // Fetch one message at a time
            e.UseConcurrencyLimit(1); // Process one message at a time
            e.ConfigureConsumer<AppointmentCreatedConsumer>(context);
        });
        // Configure saga endpoints
        cfg.ConfigureEndpoints(context);
    });
});
// Other service configurations

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
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
