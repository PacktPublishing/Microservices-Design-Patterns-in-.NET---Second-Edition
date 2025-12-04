namespace AggregatorService.Services;

public record MedicalRecordDto(
    Guid MedicalRecordId,
    Guid PatientId,
    string Diagnosis,
    DateTime RecordDate
);
public class MedicalHistoryServiceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<MedicalRecordDto>?> GetMedicalHistoryAsync(int patientId)
    {
        return await _httpClient.GetFromJsonAsync<List<MedicalRecordDto>>($"api/medicalhistory/{patientId}");
    }
}


