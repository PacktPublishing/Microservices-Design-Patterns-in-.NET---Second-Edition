using AppointmentsApi.Services;
using AppointmentsApi.Protos;

namespace AppointmentsApi.Models.DTOs;

public record AppointmentDetails(
    Guid AppointmentId,
    Patient Patient,
    Doctor Doctor,
    DateTime StartTime,
    DateTime EndTime,
    string RoomNumber,
    string Building,
    DocumentList Documents
);
    
