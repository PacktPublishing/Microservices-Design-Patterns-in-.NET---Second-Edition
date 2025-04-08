using System;
using System.Text.Json;
using AppointmentsApi.Data;
using AppointmentsApi.Models.DTOs;
using AppointmentsApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NuGet.Protocol;

namespace AppointmentsApi.Queries.GetAppointmentsByPatientId;

public class AppointmentByPatientId(Guid appointmentId, string doctorName, DateTime date)
{
    public Guid AppointmentId { get; set; } = appointmentId;
    public string DoctorName { get; set; } = doctorName;
    public DateTime Date { get; set; } = date;
}

public record GetAppointmentByPatientIdQuery(string PatientId): IRequest<List<AppointmentByPatientId>>;

public class GetAppointmentsByPatientIdHandler(EventStoreDbContext _eventStoreDbContext) : IRequestHandler<GetAppointmentByPatientIdQuery, List<AppointmentByPatientId>>
{
    public async Task<List<AppointmentByPatientId>> Handle(GetAppointmentByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var patientJson = JsonSerializer.Serialize(new { Patient = new { request.PatientId } });
        var events = await _eventStoreDbContext.Events
            .Where(e => e.EventType == "AppointmentCreatedEvent" 
                && EF.Functions.JsonContains(e.Payload, patientJson))
            .AsNoTracking()
            .ToListAsync();

        List<AppointmentByPatientId> appointments = [];
        foreach (var @event in events)
        {
            var appointment = JsonSerializer.Deserialize<AppointmentDetails>(@event.Payload);

            if (appointment != null && appointment.StartTime < DateTime.UtcNow) continue;

            appointments.Add(new AppointmentByPatientId(
                appointment.AppointmentId,
                appointment.Doctor.FirstName + " " + appointment.Doctor.LastName,
                appointment.StartTime
            ));
        }
        return appointments.OrderBy(q => q.Date).ToList();
    }
}