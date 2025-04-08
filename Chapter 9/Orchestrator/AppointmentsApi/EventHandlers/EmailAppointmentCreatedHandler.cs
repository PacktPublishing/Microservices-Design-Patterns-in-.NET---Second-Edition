using AppointmentsApi.Models.Events;
using MediatR;

namespace AppointmentsApi.EventHandlers;

public class EmailAppointmentCreatedHandler (IEmailService _emailService) : INotificationHandler<AppointmentCreatedEvent>
{
    public async Task Handle(AppointmentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Perform look ups here

        // Send Email Here
        await _emailService.SendEmailAsync(string.Empty, "Appointment Created", "Your appointment has been created");
    }
}
