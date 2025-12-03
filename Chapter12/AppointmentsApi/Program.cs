using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Services;
using Polly;
using Polly.Extensions.Http;
using MassTransit;
using Notification.Service;
using AppointmentsApi.Handlers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppointmentContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register PatientsApiClient
builder.Services.AddHttpClient<PatientsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:PatientsApi"]);
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());


// Register DoctorsApiClient
builder.Services.AddTransient<JwtAuthHandler>();
builder.Services.AddTransient<ApiKeyHandler>();

builder.Services.AddHttpClient<ExternalApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ThirdPartyApi:BaseUrl"]);
})
.AddHttpMessageHandler<ApiKeyHandler>();

builder.Services.AddHttpClient<DoctorsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:DoctorsApi"]);
})
.AddHttpMessageHandler<JwtAuthHandler>();


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
builder.Services.AddHostedService<OutboxProcessor>();


// Register the RedisCache service
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
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

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .OrResult(r => !r.IsSuccessStatusCode)
        .Or<HttpRequestException>()
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, context) => {
            // Add logic to be executed before each retry, such as logging or reauthentication
        });

}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
