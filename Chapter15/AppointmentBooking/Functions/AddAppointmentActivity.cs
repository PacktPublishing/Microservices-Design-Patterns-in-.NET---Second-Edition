using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class AddAppointmentActivity
{
    [Function(nameof(AddAppointmentActivity))]
    public async Task Run([ActivityTrigger] BookingRequest request, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(AddAppointmentActivity));
        TableClient client = TableStorage.GetClient("Appointments");
        var dateKey = request.Appointment.StartsAtUtc.ToString("yyyyMMdd");
        var entity = new AppointmentTableEntity
        {
            PartitionKey = dateKey,
            PatientEmail = request.Patient.Email,
            StartsAtUtc  = request.Appointment.StartsAtUtc,
            Duration     = request.Appointment.Duration,
            ProviderId   = request.Appointment.ProviderId,
            Location     = request.Appointment.Location
        };

        await client.AddEntityAsync(entity);
        logger.LogInformation("Appointment added for {Email} at {Start}",
            request.Patient.Email, request.Appointment.StartsAtUtc);
    }
}
