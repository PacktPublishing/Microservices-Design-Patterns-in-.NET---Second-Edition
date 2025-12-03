using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

public class BookingOrchestrator
{
    [Function(nameof(BookingOrchestrator))]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext ctx)
    {
        var request = ctx.GetInput<BookingRequest>()!;

        // Create patient
        await ctx.CallActivityAsync(nameof(AddPatientActivity), request.Patient);

        // Create appointment
        await ctx.CallActivityAsync(nameof(AddAppointmentActivity), request);

        // Fan-out: notify patient & admin
        var t1 = ctx.CallActivityAsync(nameof(SendPatientEmailActivity), request);
        var t2 = ctx.CallActivityAsync(nameof(SendAdminEmailActivity), request);
        await Task.WhenAll(t1, t2);
    }
}
