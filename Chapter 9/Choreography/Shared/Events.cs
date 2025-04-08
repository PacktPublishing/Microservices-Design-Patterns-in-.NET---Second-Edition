namespace Shared.Events;

public record AppointmentCreated(Guid AppointmentId, Guid PatientId, DateTime AppointmentDate);
public record PaymentProcessed(Guid AppointmentId, Guid PatientId, decimal Amount, bool Success);
