namespace AppointmentsApi.Services;
public record Doctor(
    Guid DoctorId,
    string FirstName,
    string LastName,
    string Specialty
);

public class DoctorsApiClient(HttpClient _httpClient)
{
    // Example method to retrieve doctor details
    public async Task<Doctor> GetDoctorAsync(Guid doctorId)
    {
        var response = await _httpClient.GetAsync($"/api/doctors/{doctorId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Doctor>();
    }
}
