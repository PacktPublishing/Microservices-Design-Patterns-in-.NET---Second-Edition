using Aspire.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL server
var postgres = builder.AddPostgres("pg");

// Databases for each service
var patientsDb = postgres.AddDatabase("patientsdb");
var appointmentsDb = postgres.AddDatabase("appointmentsdb");

// Projects
var patients = builder.AddProject<Projects.HealthcareAspire_PatientsApi>("patientsapi")
                      .WithReference(patientsDb);
var appointments = builder.AddProject<Projects.HealthcareAspire_AppointmentsApi>("appointmentsapi")
                          .WithReference(appointmentsDb)
                          .WithReference(patients); // injects discovery info for "patientsapi"

builder.Services.AddHttpClient("patients",
    c => c.BaseAddress = new Uri("https://patientsapi"));

builder.Build().Run();
