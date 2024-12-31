using System.Collections.Concurrent;
using AppointmentsApi.Models.Messages;
using AppointmentsApi.Services;
using MassTransit;

namespace Notification.Service;

public class AppointmentCreatedConsumer(IEmailService _emailService, PatientsApiClient patientsApiClient, DoctorsApiClient doctorsApiClient) : IConsumer<AppointmentCreated>
{
    // Dictionary to track the last processed timestamp for each appointment
    private static readonly ConcurrentDictionary<Guid, DateTime> LastProcessedTimestamps = new();
    private static readonly ConcurrentDictionary<Guid, bool> ProcessedMessageIds = new();

    public async Task Consume(ConsumeContext<AppointmentCreated> context)
    {
        var message = context.Message;

        // Check if the message has already been processed
        if (ProcessedMessageIds.ContainsKey(message.MessageId))
        {
            Console.WriteLine($"Duplicate message detected: {message.MessageId}. Ignoring.");
            return; // Exit without processing
        }

        // Retrieve the last processed timestamp for this appointment
        var lastTimestamp = LastProcessedTimestamps.GetOrAdd(message.AppointmentId, DateTime.MinValue);

        // Check if the message is newer than the last processed message
        if (message.Timestamp > lastTimestamp)
        {

            Console.WriteLine($"Retrieve Doctor details");
            //var doctor = await doctorsApiClient.GetDoctorAsync(message.DoctorId);

            Console.WriteLine($"Retrieve patient details");
            Console.WriteLine($"Send Email to patient ");
            //var patient = await patientsApiClient.GetPatientAsync(message.PatientId);

            //  var emailContent = $"Dear {patient.FirstName} {patient.LastName},\n\n" +
            //                    $"Your appointment with Dr. {doctor.FirstName} {doctor.LastName} is confirmed for {message.AppointmentDate}.\n\n" +
            //                    "Best regards,\nHealthCare Management System";

            // await _emailService.SendEmailAsync(patient.Email, "Appointment Confirmation", emailContent);   
            
            // Mark the message as processed
            ProcessedMessageIds[message.MessageId] = true;
        
        }
        else
        {
            // implement logic to handle out-of-order messages, such as logging or storing for later processing
        }
    }
}

