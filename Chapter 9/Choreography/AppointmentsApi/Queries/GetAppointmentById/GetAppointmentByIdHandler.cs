using System.Text.Json;
using AppointmentsApi.Data;
using AppointmentsApi.Models.DTOs;
using AppointmentsApi.Protos;
using AppointmentsApi.Services;
using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsApi.Queries.GetAppointmentById;

public class GetAppointmentByIdHandler(EventStoreDbContext _eventStoreDbContext) : IRequestHandler<GetAppointmentByIdQuery, AppointmentDetails>
{
    public async Task<AppointmentDetails> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointmentEvent = await _eventStoreDbContext.Events
        .Where(e => e.AggregateId.ToString() == request.Id)
        .OrderByDescending(e => e.EventTimestamp)
        .FirstOrDefaultAsync();

        // Additional logic required to parse data based on the event type and potentially modified payload format

        // return serialized payload data
        return appointmentEvent == null ? null : JsonSerializer.Deserialize<AppointmentDetails>(appointmentEvent.Payload);;
    }
}
