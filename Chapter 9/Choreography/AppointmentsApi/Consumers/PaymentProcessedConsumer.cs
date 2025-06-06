using MassTransit;
using Shared.Events;

namespace AppointmentsApi.Consumers;

public class PaymentProcessedConsumer(AppointmentContext _dbContext) : IConsumer<PaymentProcessed>
{
    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var message = context.Message;
        Console.WriteLine(
            $"[Appointment] CorrelationId={message.CorrelationId}: Payment " +
            $"{(message.Success ? "succeeded" : "failed")} for Appointment {message.AppointmentId}");

        if (!message.Success)
        {
            var appointment = await _dbContext.Appointments.FindAsync(message.AppointmentId);
            if (appointment != null)
            {
                appointment.IsCanceled = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
