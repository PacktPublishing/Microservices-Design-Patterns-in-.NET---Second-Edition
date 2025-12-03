using AppointmentsApi.Models.DTOs;
using AppointmentsApi.Protos;
using AppointmentsApi.Services;
using Grpc.Net.Client;
using MediatR;

namespace AppointmentsApi.Queries.GetAppointmentById;

public class GetAppointmentByIdHandler(AppointmentContext _context, PatientsApiClient _patientsApiClient, DoctorsApiClient _doctorsApiClient) : IRequestHandler<GetAppointmentByIdQuery, AppointmentDetails>
{
    public async Task<AppointmentDetails> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments.FindAsync(request.Id);

        if (appointment == null)
        {
            return null;
        }

        // Use the clients to fetch patient and doctor details
        var patient = await _patientsApiClient.GetPatientAsync(appointment.PatientId);
        var doctor = await _doctorsApiClient.GetDoctorAsync(appointment.DoctorId);

        // Use GRPC service to retrieve document information on patient
        using var channel = GrpcChannel.ForAddress("http://localhost:5170");
        var client = new DocumentService.DocumentServiceClient(channel);
        var documents = await client.GetAllAsync(new PatientId { Id = patient.PatientId.ToString() });

        // Combine data and return response
        AppointmentDetails appointmentDetails = new AppointmentDetails(

            appointment.AppointmentId,
            patient,
            doctor,
            appointment.Slot.Start,
            appointment.Slot.End,
            appointment.Location.RoomNumber,
            appointment.Location.Building,
            documents
        );

        return appointmentDetails;
    }
}
