using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
public class StartBookingHttp
{
    private const string QueueName = "appointments";
    private readonly QueueClient _queue;
    private bool isValid = true;

    public StartBookingHttp()
    {
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        // Ensure consistent Base64 message encoding
        var options = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };
        _queue = new QueueClient(conn, QueueName, options);
        _queue.CreateIfNotExists();
    }

    [Function("StartBookingHttp")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext ctx)
    {
        BookingRequest? payload;
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            payload = JsonSerializer.Deserialize<BookingRequest>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return await BadRequest(req, "Malformed JSON.");
        }

        if (payload == null)
        {
            return await BadRequest(req, "Invalid request payload.");
        }

        if (string.IsNullOrWhiteSpace(payload.Patient.FirstName)) isValid = false;
        if (string.IsNullOrWhiteSpace(payload.Patient.LastName))  isValid = false;
        if (!isValid) return await BadRequest(req, $"Validation failed");

        var json = JsonSerializer.Serialize(payload);
        await _queue.SendMessageAsync(json);
        var ok = req.CreateResponse(HttpStatusCode.Accepted);
        await ok.WriteStringAsync("Booking request accepted and queued for processing.");
        
        return ok;
    }

    private static async Task<HttpResponseData> BadRequest(HttpRequestData req, string msg)
    {
        var res = req.CreateResponse(HttpStatusCode.BadRequest);
        await res.WriteStringAsync(msg);
        return res;
    }
}
