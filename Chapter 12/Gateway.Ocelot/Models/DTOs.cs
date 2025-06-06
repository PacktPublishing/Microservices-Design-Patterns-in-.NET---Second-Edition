namespace Gateway.Ocelot.Models;

public record DoctorDto(
Guid DoctorId,
string FirstName,
string LastName,
string Specialty
);

public record PatientDto(
    Guid PatientId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string ContactNumber,
    string Email
);

public record AppointmentDetailsDto(
    Guid AppointmentId,
    PatientDto Patient,
    DoctorDto Doctor,
    DateTime StartTime,
    DateTime EndTime,
    string RoomNumber,
    string Building
);