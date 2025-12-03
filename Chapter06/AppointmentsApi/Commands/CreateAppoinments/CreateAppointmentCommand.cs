using System;
using AppointmentsApi.Data;
using MediatR;

namespace AppointmentsApi.Commands.CreateAppoinments;

public record CreateAppointmentCommand(Guid DoctorId, Guid PatientId, Location Location, TimeSlot Slot, string Purpose) : IRequest<Appointment>;