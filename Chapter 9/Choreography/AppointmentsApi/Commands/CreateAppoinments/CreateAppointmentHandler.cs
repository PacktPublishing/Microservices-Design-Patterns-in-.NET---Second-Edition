using System;
using AppointmentsApi.Data;
using AppointmentsApi.Models.Messages;
using MassTransit;
using MediatR;

namespace AppointmentsApi.Commands.CreateAppoinments;

public class CreateAppointmentHandler(AppointmentContext _context, IPublishEndpoint _publishEndpoint) : IRequestHandler<CreateAppointmentCommand, Appointment>
{
    public async Task<Appointment> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Handle Pre-checks and Validations Here
        var newAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            Location = request.Location,
            Slot = request.Slot,
            Purpose = request.Purpose
        };

        _context.Add(newAppointment);
        await _context.SaveChangesAsync();
        
	    //Perform post-creation hand-off to services bus.
        var appointmentCreatedDetails = new AppointmentCreated
        {
            AppointmentId = newAppointment.AppointmentId,
            DoctorId = newAppointment.DoctorId,
            PatientId = newAppointment.PatientId,
            AppointmentDate = newAppointment.Slot.Start,
            Timestamp = DateTime.UtcNow,
            MessageId = newAppointment.AppointmentId
        };
        
        await _publishEndpoint.Publish<AppointmentCreated>(new
        {
            newAppointment.AppointmentId,
            newAppointment.PatientId,
            newAppointment.DoctorId,
            newAppointment.Slot.Start,
            DateTime.UtcNow,
            MessageId = newAppointment.AppointmentId
        }, cancellationToken);


        return newAppointment;
    }
}
