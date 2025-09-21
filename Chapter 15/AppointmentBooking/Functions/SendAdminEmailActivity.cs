using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class SendAdminEmailActivity
{
    [Function(nameof(SendAdminEmailActivity))]
    public async Task Run([ActivityTrigger] BookingRequest request, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(SendAdminEmailActivity));
        // Send Email logic
        logger.LogInformation("Admin email sent for appointment: Patient {Email} on {Date}", 
            request.Patient.Email, request.Appointment.StartsAtUtc);
        await Task.CompletedTask;
    }
}
