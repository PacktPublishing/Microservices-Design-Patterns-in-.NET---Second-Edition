using Microsoft.EntityFrameworkCore;

namespace AppointmentsApi.Models;

/// <summary>
/// Value Object representing a time slot for an appointment
/// </summary>
[Owned]
public record TimeSlot
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public TimeSlot(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End time must be after start time.");
        }
        
        Start = start;
        End = end;
    }

    public TimeSpan Duration => End - Start;
}

/// <summary>
/// Value Object representing the location of an appointment
/// </summary>
[Owned]
public record Location(string RoomNumber, string Building);

/// <summary>
/// Aggregate Root: Appointment
/// Represents a scheduled medical appointment with business rules and domain logic
/// </summary>
public class Appointment
{
    public Guid AppointmentId { get; }
    public Guid PatientId { get; }
    public Guid DoctorId { get; }
    public TimeSlot Slot { get; private set; } = null!;
    public Location Location { get; } = null!;
    public string Purpose { get; private set; } = string.Empty;

    // EF Core constructor
    private Appointment() { }

    public Appointment(Guid appointmentId, Guid patientId, Guid doctorId, TimeSlot slot, Location location, string purpose)
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorId = doctorId;
        Slot = slot;
        Location = location;
        Purpose = purpose;
    }

    /// <summary>
    /// Domain behavior: Reschedule the appointment to a new time slot
    /// </summary>
    public void Reschedule(TimeSlot newSlot)
    {
        // Business rules to check for conflicts, etc.
        Slot = newSlot;
    }

    /// <summary>
    /// Domain behavior: Change the purpose of the appointment
    /// </summary>
    public void ChangePurpose(string newPurpose)
    {
        Purpose = newPurpose;
    }
}
