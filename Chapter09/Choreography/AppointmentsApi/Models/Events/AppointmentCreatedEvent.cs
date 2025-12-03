using AppointmentsApi.Data;
using MediatR;

namespace AppointmentsApi.Models.Events;

public class AppointmentCreatedEvent : INotification
{
    public Guid CorrelationId { get; set; }
    public Guid AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public TimeSlot Slot { get; set; }
    public Location Location { get; set; }
}