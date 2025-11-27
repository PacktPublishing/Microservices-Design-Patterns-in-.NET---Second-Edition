// Models/AppointmentTableEntity.cs
using Azure;
using Azure.Data.Tables;

public class AppointmentTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N");
    public string PatientEmail { get; set; } = default!;
    public DateTime StartsAtUtc { get; set; }
    public TimeSpan Duration { get; set; }
    public string ProviderId { get; set; } = default!;
    public string? Location { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}