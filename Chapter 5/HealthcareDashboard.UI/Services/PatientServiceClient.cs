using System;
using System.Net.Http.Json;

namespace HealthcareDashboard.UI.Services;

public record Patient(
    Guid PatientId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string ContactNumber,
    string Email
);

public class PatientServiceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Patient?> GetPatientAsync(Guid patientId)
    {
        return await _httpClient.GetFromJsonAsync<Patient>($"/{patientId}");
    }
}

