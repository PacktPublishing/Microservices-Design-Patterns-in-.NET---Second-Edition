namespace AppointmentsApi.Models;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredOnUtc { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
}
