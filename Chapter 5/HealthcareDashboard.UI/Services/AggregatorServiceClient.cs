using System.Net.Http.Json;
using HealthcareDashboard.UI.Models;

namespace HealthcareDashboard.UI.Services;

public class AggregatorServiceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<PatientDashboardModel?> GetPatientDashboardAsync(Guid patientId)
    {
        return await _httpClient.GetFromJsonAsync<PatientDashboardModel?>($"/aggregator/patient-dashboard/{patientId}");
    }
}

