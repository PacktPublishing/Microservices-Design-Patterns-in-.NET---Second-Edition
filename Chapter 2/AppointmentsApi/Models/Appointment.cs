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
            throw new ArgumentException("End time must be after start time");
        
        Start = start;
        End = end;
    }

    public TimeSpan Duration => End - Start;
}

/// <summary>
/// Value Object representing the location of an appointment
/// </summary>
[Owned]
public record Location
{
    public string RoomNumber { get; init; }
    public string Building { get; init; }

    public Location(string roomNumber, string building)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Room number cannot be empty", nameof(roomNumber));
        
        if (string.IsNullOrWhiteSpace(building))
            throw new ArgumentException("Building cannot be empty", nameof(building));
        
        RoomNumber = roomNumber;
        Building = building;
    }
}

/// <summary>
/// Aggregate Root: Appointment
/// Represents a scheduled medical appointment with business rules and domain logic
/// </summary>
public class Appointment
{
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public TimeSlot Slot { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public string Purpose { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }

    // EF Core constructor
    private Appointment() { }

    public Appointment(Guid patientId, Guid doctorId, TimeSlot slot, Location location, string purpose)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
        
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));
        
        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be empty", nameof(purpose));

        AppointmentId = Guid.NewGuid();
        PatientId = patientId;
        DoctorId = doctorId;
        Slot = slot ?? throw new ArgumentNullException(nameof(slot));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Purpose = purpose;
        Status = AppointmentStatus.Scheduled;
    }

    /// <summary>
    /// Domain behavior: Reschedule the appointment to a new time slot
    /// </summary>
    public void Reschedule(TimeSlot newSlot)
    {
        if (newSlot == null)
            throw new ArgumentNullException(nameof(newSlot));
        
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot reschedule a cancelled appointment");
        
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot reschedule a completed appointment");

        Slot = newSlot;
    }

    /// <summary>
    /// Domain behavior: Change the purpose of the appointment
    /// </summary>
    public void ChangePurpose(string newPurpose)
    {
        if (string.IsNullOrWhiteSpace(newPurpose))
            throw new ArgumentException("Purpose cannot be empty", nameof(newPurpose));
        
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot change purpose of a cancelled appointment");

        Purpose = newPurpose;
    }

    /// <summary>
    /// Domain behavior: Relocate the appointment
    /// </summary>
    public void Relocate(Location newLocation)
    {
        if (newLocation == null)
            throw new ArgumentNullException(nameof(newLocation));
        
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot relocate a cancelled appointment");
        
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot relocate a completed appointment");

        Location = newLocation;
    }

    /// <summary>
    /// Domain behavior: Confirm the appointment
    /// </summary>
    public void Confirm()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot confirm a cancelled appointment");
        
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Appointment is already completed");

        Status = AppointmentStatus.Confirmed;
    }

    /// <summary>
    /// Domain behavior: Cancel the appointment
    /// </summary>
    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed appointment");

        Status = AppointmentStatus.Cancelled;
    }

    /// <summary>
    /// Domain behavior: Mark the appointment as completed
    /// </summary>
    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled appointment");

        Status = AppointmentStatus.Completed;
    }

    /// <summary>
    /// Domain query: Check if the appointment is in the past
    /// </summary>
    public bool IsPastAppointment()
    {
        return Slot.End < DateTime.UtcNow;
    }

    /// <summary>
    /// Domain query: Check if the appointment conflicts with another time slot
    /// </summary>
    public bool ConflictsWith(TimeSlot otherSlot)
    {
        if (otherSlot == null)
            throw new ArgumentNullException(nameof(otherSlot));

        return Slot.Start < otherSlot.End && otherSlot.Start < Slot.End;
    }
}

/// <summary>
/// Enumeration representing the status of an appointment
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}
