// Models/Appointment.cs
public record Appointment(DateTime StartsAtUtc,
    TimeSpan Duration, string ProviderId, string? Location = null );

