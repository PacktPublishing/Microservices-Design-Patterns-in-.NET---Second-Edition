using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults: telemetry, health checks, discovery, resilience
builder.AddServiceDefaults();

// Add minimal services
builder.Services.AddEndpointsApiExplorer();

// Bind Postgres connection string via Aspire (Uses connection name "appointmentsdb")
var connString = builder.Configuration.GetConnectionString("appointmentsdb")
                 ?? builder.Configuration["ConnectionStrings:appointmentsdb"];

if (!string.IsNullOrWhiteSpace(connString))
{
    builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connString));
}

var app = builder.Build();

app.MapGet("/appointments", () => Results.Ok(new[] { new { id = 1, date = DateTime.UtcNow, patientId = 123 } }))
   .WithName("GetAppointments");

app.MapGet("/appointments/dbping", async (NpgsqlDataSource? ds) =>
{
    if (ds is null) return Results.Problem("Database not configured");
    await using var conn = await ds.OpenConnectionAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "select 1";
    var r = await cmd.ExecuteScalarAsync();
    return Results.Ok(new { ok = (r is int i && i == 1) });
});

app.MapDefaultEndpoints();

app.Run();
