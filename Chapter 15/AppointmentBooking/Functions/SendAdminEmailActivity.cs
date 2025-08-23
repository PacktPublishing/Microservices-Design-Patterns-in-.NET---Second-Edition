using Microsoft.Azure.Functions.Worker;

public class SendAdminEmailActivity
{
    [Function(nameof(SendAdminEmailActivity))]
    public async Task Run([ActivityTrigger] BookingRequest request, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(SendAdminEmailActivity));
	  // Send Email logic
    }
}
