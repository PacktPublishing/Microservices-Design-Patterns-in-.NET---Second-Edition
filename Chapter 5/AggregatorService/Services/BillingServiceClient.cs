namespace AggregatorService.Services;

public record BillingDetailDto(
    Guid BillingDetailId,
    Guid PatientId,
    decimal Amount,
    DateTime BillingDate
);
public class BillingServiceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<BillingDetailDto>?> GetBillingDetailsAsync(int patientId)
    {
        return await _httpClient.GetFromJsonAsync<List<BillingDetailDto>>($"api/billing/{patientId}");
    }
}
