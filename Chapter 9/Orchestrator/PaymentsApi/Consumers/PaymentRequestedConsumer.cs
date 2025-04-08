using MassTransit;
using Shared.Events;

namespace PaymentsApi.Consumers;

public class PaymentRequestedConsumer : IConsumer<PaymentRequested>
{
    public async Task Consume(ConsumeContext<PaymentRequested> context)
    {
        var success = new Random().Next(0, 2) == 1;
        Console.WriteLine($"[PaymentService] Payment requested for Appointment {context.Message.AppointmentId}. Success? {success}");

        if (success)
        {
            await context.Publish(new PaymentSucceeded(context.Message.AppointmentId));
        }
        else
        {
            await context.Publish(new PaymentFailed(context.Message.AppointmentId));
        }
    }
}
