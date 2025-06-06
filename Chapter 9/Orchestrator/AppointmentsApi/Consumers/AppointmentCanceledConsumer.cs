using MassTransit;
using Shared.Events;

namespace AppointmentsApi.Consumers;

public class AppointmentCanceledConsumer : IConsumer<AppointmentCanceled>
{
    private readonly AppointmentContext _dbContext;

    public AppointmentCanceledConsumer(AppointmentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AppointmentCanceled> context)
    {
        var appointment = await _dbContext.Appointments.FindAsync(context.Message.AppointmentId);
        if (appointment != null)
        {
            appointment.IsCanceled = true;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"[AppointmentService] Appointment {appointment.AppointmentId} canceled.");
        }
    }
}
