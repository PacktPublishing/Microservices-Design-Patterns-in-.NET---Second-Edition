using Microsoft.Azure.Functions.Worker;

public class SendPatientEmailActivity
{
    [Function(nameof(SendPatientEmailActivity))]
    public async Task Run([ActivityTrigger] BookingRequest request, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(SendPatientEmailActivity));
	  // Send Email logic
    }
}