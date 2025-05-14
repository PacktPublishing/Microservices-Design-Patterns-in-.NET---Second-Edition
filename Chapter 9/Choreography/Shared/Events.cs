namespace Shared.Events;

public record AppointmentCreated(Guid CorrelationId, Guid AppointmentId, Guid PatientId, DateTime AppointmentDate);
public record PaymentProcessed(Guid CorrelationId, Guid AppointmentId, Guid PatientId, decimal Amount, bool Success);
