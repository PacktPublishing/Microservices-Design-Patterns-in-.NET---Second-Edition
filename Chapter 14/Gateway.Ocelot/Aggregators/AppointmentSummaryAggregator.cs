using Gateway.Ocelot.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Text;

namespace Gateway.Ocelot.Agggregtors;
public sealed class AppointmentSummaryAggregator : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> contexts)
    {
        var appointment = await contexts[0].Items.DownstreamResponse()
                                    .Content.ReadFromJsonAsync<AppointmentDetailsDto>();

        var patient = await contexts[1].Items.DownstreamResponse()
                                 .Content.ReadFromJsonAsync<PatientDto>();

        var doctor = await contexts[2].Items.DownstreamResponse()
                                .Content.ReadFromJsonAsync<DoctorDto>();

        var summary = new
        {
            appointment.AppointmentId,
            appointment.StartTime,
            Doctor = new { doctor.DoctorId, doctor.FirstName, doctor.LastName, doctor.Specialty },
            Patient = new { patient.PatientId, patient.FirstName, patient.LastName, patient.Gender }
        };

        var json = JsonConvert.SerializeObject(summary);
        var headers = new List<Header> { new("Content-Type", ["application/json"]) };
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        return new DownstreamResponse(httpContent,
                                      HttpStatusCode.OK,
                                      headers,
                                      "OK");
    }
}
