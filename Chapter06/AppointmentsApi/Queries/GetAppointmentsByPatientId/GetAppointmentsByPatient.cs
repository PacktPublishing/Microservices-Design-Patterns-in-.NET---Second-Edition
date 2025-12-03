using System;
using AppointmentsApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsApi.Queries.GetAppointmentsByPatientId;

public class AppointmentByPatientId(Guid appointmentId, string doctorName, DateTime date)
{
    public Guid AppointmentId { get; set; } = appointmentId;
    public string DoctorName { get; set; } = doctorName;
    public DateTime Date { get; set; } = date;
}

public record GetAppointmentByPatientIdQuery(string PatientId): IRequest<List<AppointmentByPatientId>>;

public class GetAppointmentsByPatientIdhandler(AppointmentContext _context, DoctorsApiClient _doctorsApiClient) : IRequestHandler<GetAppointmentByPatientIdQuery, List<AppointmentByPatientId>>
{
    public async Task<List<AppointmentByPatientId>> Handle(GetAppointmentByPatientIdQuery request, CancellationToken cancellationToken)
    {
        // Get appointments for a patient
        var appointments = await _context.Appointments.Where(a => a.PatientId.ToString() == request.PatientId)
        .Select(q => new AppointmentByPatientId(q.AppointmentId, string.Empty, q.Slot.Start))
        .ToListAsync();

        // Get doctor details for each appointment in parallel
        var tasks = appointments.Select(async appointment =>
        {
            var doctor = await _doctorsApiClient.GetDoctorAsync(appointment.AppointmentId);
            appointment.DoctorName = doctor.LastName;
        });

        await Task.WhenAll(tasks);
        
        return appointments;
    }
}