namespace AppointmentsApi.Services;
public record Patient(
    Guid PatientId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string ContactNumber,
    string Email
);

public class PatientsApiClient
{
    private readonly HttpClient _httpClient;

    public PatientsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Example method to retrieve patient details
    public async Task<Patient> GetPatientAsync(Guid patientId)
    {
        var response = await _httpClient.GetAsync($"/api/patients/{patientId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Patient>();
    }
}
