using AggregatorService.Models;

namespace AggregatorService.Services;

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

    public async Task<PatientDto?> GetPatientAsync(int patientId)
    {
        var patient = await _httpClient.GetFromJsonAsync<Patient>($"api/patients/{patientId}");
        if(patient is null) return null;

        var patientDto = new PatientDto
        {
            Id = patient.PatientId,
            Fullname = $"{patient.FirstName} {patient.LastName}",
            DateOfBirth = patient.DateOfBirth,
            Email = patient.Email,
            ContactNumber = patient.ContactNumber,
            Gender = patient.Gender
        };

        return patientDto;
    }
}
