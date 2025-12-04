using MassTransit;
using Shared.Events;

namespace PaymentsApi.Consumers;

public class AppointmentCreatedConsumer : IConsumer<AppointmentCreated>
{
    public async Task Consume(ConsumeContext<AppointmentCreated> context)
    {
        var message = context.Message;
        Console.WriteLine($"CorrelationId={message.CorrelationId}: Payment | Processing payment for Appointment ID: {message.AppointmentId}");

        // Simulate payment failure 50% of the time
        var paymentSuccess = new Random().Next(0, 2) == 1;

        await context.Publish(new PaymentProcessed(message.CorrelationId, message.AppointmentId, message.PatientId, 100.00m, paymentSuccess));
    }
}