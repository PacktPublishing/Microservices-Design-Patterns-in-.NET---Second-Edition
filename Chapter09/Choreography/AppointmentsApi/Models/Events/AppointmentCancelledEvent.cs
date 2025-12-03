using MediatR;

namespace AppointmentsApi.Models.Events;

public class AppointmentCancelledEvent : INotification
{
    public Guid CorrelationId { get; set; }
    public Guid AppointmentId { get; set; }
    public string Reason { get; set; }
    public DateTime CancelledAt { get; set; }
}