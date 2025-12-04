using System;
using AggregatorService.Models;

namespace AggregatorService.Services;

public class AppointmentsServiceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<AppointmentDto>?> GetAppointmentsAsync(int patientId)
    {
        try
        {
            var appointments = await _httpClient.GetFromJsonAsync<List<AppointmentDto>?>($"api/appointmentsforpatient/{patientId}");
            return appointments;
        }
        catch
        {
            return null;
        }

    }
}
