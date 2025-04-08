using System;
using AppointmentsApi.Data;
using MediatR;

namespace AppointmentsApi.Queries.GetAppointments;

public record GetAppointmentsQuery() : IRequest<List<Appointment>>;
