using System;

namespace AppointmentsApi.Services;

public class ExternalApiService(HttpClient _httpClient)
{
    public async Task<string> GetSecureDataAsync()
    {
        var response = await _httpClient.GetAsync("/secure-data");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}

