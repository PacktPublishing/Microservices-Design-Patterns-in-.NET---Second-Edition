using AppointmentsApi.Models.DTOs;
using MediatR;

namespace AppointmentsApi.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(string Id): IRequest<AppointmentDetails>;
