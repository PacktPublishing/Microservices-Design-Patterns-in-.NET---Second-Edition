using MediatR;

namespace AppointmentsApi.Models.Events;

public class AppointmentCancelledEvent : INotification
{
    public Guid AppointmentId { get; set; }
    public string Reason { get; set; }
    public DateTime CancelledAt { get; set; }
}