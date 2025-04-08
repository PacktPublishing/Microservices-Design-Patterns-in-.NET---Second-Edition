using System.Text.Json;
using AppointmentsApi.Data;
using AppointmentsApi.Models.DTOs;
using AppointmentsApi.Models.Events;
using AppointmentsApi.Protos;
using AppointmentsApi.Services;
using Grpc.Net.Client;
using MediatR;

namespace AppointmentsApi.EventHandlers;

public class SaveAppointmentCreatedEventDetailsHandler (EventStoreDbContext _eventStoreDbContext,PatientsApiClient _patientsApiClient, DoctorsApiClient _doctorsApiClient) : INotificationHandler<AppointmentCreatedEvent>
{
    public async Task Handle(AppointmentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Use the clients to fetch patient and doctor details
        //var patient = await _patientsApiClient.GetPatientAsync(notification.PatientId);
        //var doctor = await _doctorsApiClient.GetDoctorAsync(notification.DoctorId);

        var patient = new Patient(Guid.NewGuid(), "Test", "Patient", DateTime.Now, "Male", "123456789", "email@test.com");
        var doctor = new Doctor(Guid.NewGuid(), "Test", "Doctor", "Medicine");

        // Use GRPC service to retrieve document information on patient
        //using var channel = GrpcChannel.ForAddress("http://localhost:5170");
        //var client = new DocumentService.DocumentServiceClient(channel);
        //var documents = await client.GetAllAsync(new PatientId { Id = patient.PatientId.ToString() });

        var documents = new DocumentList();

        AppointmentDetails appointmentDetails = new (

            notification.AppointmentId,
            patient,
            doctor,
            notification.Slot.Start,
            notification.Slot.End,
            notification.Location.RoomNumber,
            notification.Location.Building,
            documents
        );

        // Save the details to the event store
        EventEntity @event = new ()
        {
            AggregateId = notification.AppointmentId,
            EventId = Guid.NewGuid(),
            EventType = nameof(AppointmentCreatedEvent),
            Payload = JsonSerializer.Serialize(appointmentDetails),
            EventTimestamp = DateTime.UtcNow
        };

        _eventStoreDbContext.Events.Add(@event);
        await _eventStoreDbContext.SaveChangesAsync(cancellationToken); 
    }
}