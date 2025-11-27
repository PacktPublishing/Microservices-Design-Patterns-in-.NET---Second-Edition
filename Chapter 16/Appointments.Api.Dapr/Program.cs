using Dapr.Client;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();

var serviceName = "appointments-service";
var endpoint = "http://localhost:4318";

builder.Services.AddOpenTelemetry()
  .ConfigureResource(rb => rb
      .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
      .AddAttributes(
      [
          new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)
      ]))
  .WithTracing(t => t
      .AddAspNetCoreInstrumentation(o => o.RecordException = true)
      .AddHttpClientInstrumentation()
      .AddSource("Appointments.Domain")
      .AddOtlpExporter(o => o.Endpoint = new Uri(endpoint)))
  .WithMetrics(m => m
      .AddAspNetCoreInstrumentation()
      .AddHttpClientInstrumentation()
      .AddOtlpExporter(o => o.Endpoint = new Uri(endpoint)));


var app = builder.Build();
app.MapGet("/api/appointments/health", () => Results.Ok(new { status = "ok" }));
// Call Notifications through Dapr service invocation
app.MapPost("/api/appointments/{id:guid}/notify", async (Guid id, DaprClient dapr) =>
{
    var payload = new { appointmentId = id };
    await dapr.InvokeMethodAsync(HttpMethod.Post, "notifications", "api/notify", payload);
    return Results.Accepted();
});
// Publish an event
app.MapPost("/api/appointments/{id:guid}/publish", async (Guid id, DaprClient dapr) =>
{
    await dapr.PublishEventAsync("pubsub", "appointment.created", new { appointmentId = id });
    return Results.Accepted();
});
app.Run();
