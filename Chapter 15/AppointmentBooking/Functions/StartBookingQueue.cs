using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

public class StartBookingQueue
{
    [Function("StartBookingQueue")]
    public async Task Run(
        [QueueTrigger("appointments")] string message,
        [DurableClient] DurableTaskClient client,
        ILogger<StartBookingQueue> logger,
        FunctionContext ctx)
    {
        BookingRequest? request = null;
        try
        {
            request = JsonSerializer.Deserialize<BookingRequest>(message,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize booking message.");
            return;
        }

        if (request is null)
        {
            logger.LogError("Null booking request.");
            return;
        }
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(BookingOrchestrator), request);
        logger.LogInformation("Orchestration started: {InstanceId}", instanceId);
    }
}
