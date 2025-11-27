// Models/BookingRequest.cs
// The HTTP contract we accept and pass through the queue/orchestrator.
public record BookingRequest(Patient Patient,     Appointment Appointment);

