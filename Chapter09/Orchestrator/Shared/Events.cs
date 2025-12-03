namespace Shared.Events;

public record AppointmentCreated(Guid AppointmentId, Guid PatientId, DateTime AppointmentDate);
public record PaymentProcessed(Guid AppointmentId, Guid PatientId, decimal Amount, bool Success);

// Command to request appointment booking
public record AppointmentRequested(Guid AppointmentId, Guid PatientId, DateTime AppointmentDate);

// Command from saga orchestrator to payment service
public record PaymentRequested(Guid AppointmentId, decimal Amount);

// Events published by payment service
public record PaymentSucceeded(Guid AppointmentId);
public record PaymentFailed(Guid AppointmentId);

// Saga concluding events
public record AppointmentConfirmed(Guid AppointmentId);
public record AppointmentCanceled(Guid AppointmentId);
