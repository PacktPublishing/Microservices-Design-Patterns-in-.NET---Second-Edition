using AppointmentsApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsApi.Queries.GetAppointments;

public class GetAppointmentsHandler(AppointmentContext _context) : IRequestHandler<GetAppointmentsQuery, List<Appointment>>
{
    public async Task<List<Appointment>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Appointments.ToListAsync();
    }
}
