using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class SendPatientEmailActivity
{
    [Function(nameof(SendPatientEmailActivity))]
    public async Task Run([ActivityTrigger] BookingRequest request, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(SendPatientEmailActivity));
        // Send Email logic
        logger.LogInformation("Patient email sent to {Email} for appointment on {Date}", 
            request.Patient.Email, request.Appointment.StartsAtUtc);
        await Task.CompletedTask;
    }
}