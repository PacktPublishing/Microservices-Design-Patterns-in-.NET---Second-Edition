using System;

namespace AppointmentsApi.Models.Messages;

public class AppointmentCreated
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid MessageId { get; set; }
}
