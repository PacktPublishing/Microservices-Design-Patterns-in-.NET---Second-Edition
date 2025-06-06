using MassTransit;
using Shared.Data;
using Shared.Events;

namespace Shared.SagaOrchestrator;

public class BookingStateMachine : MassTransitStateMachine<BookingSagaState>
{
    public BookingStateMachine()
    {
        // Define the states
        InstanceState(x => x.CurrentState);

        Event(() => AppointmentRequestedEvent, x =>
        {
            x.CorrelateById(ctx => ctx.Message.AppointmentId);
            x.SelectId(ctx => ctx.Message.AppointmentId); // Saga Key
        });

        Event(() => PaymentSucceededEvent, x => x.CorrelateById(ctx => ctx.Message.AppointmentId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.AppointmentId));

        Initially(
            When(AppointmentRequestedEvent)
                .Then(ctx =>
                {
                    ctx.Instance.AppointmentId = ctx.Data.AppointmentId;
                    Console.WriteLine($"[Saga] Received AppointmentRequested for {ctx.Data.AppointmentId}");
                })
                .TransitionTo(BookingInitiated)
                .Publish(ctx => new PaymentRequested(ctx.Data.AppointmentId, 99.99m)) // Request Payment
        );

        During(BookingInitiated,
            When(PaymentSucceededEvent)
                .Then(ctx =>
                {
                    Console.WriteLine($"[Saga] Payment succeeded for {ctx.Instance.AppointmentId}");
                })
                .Publish(ctx => new AppointmentConfirmed(ctx.Instance.AppointmentId))
                .TransitionTo(BookingCompleted)
                .Finalize(),

            When(PaymentFailedEvent)
                .Then(ctx =>
                {
                    Console.WriteLine($"[Saga] Payment failed for {ctx.Instance.AppointmentId}");
                })
                .Publish(ctx => new AppointmentCanceled(ctx.Instance.AppointmentId))
                .TransitionTo(BookingFailed)
                .Finalize()
        );

        // Set the states as final for cleanup
        SetCompletedWhenFinalized();
    }

    // States
    public State BookingInitiated { get; private set; }
    public State BookingCompleted { get; private set; }
    public State BookingFailed { get; private set; }

    // Events
    public Event<AppointmentRequested> AppointmentRequestedEvent { get; private set; }
    public Event<PaymentSucceeded> PaymentSucceededEvent { get; private set; }
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
}
