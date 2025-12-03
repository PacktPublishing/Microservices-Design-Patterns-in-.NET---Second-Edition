using AppointmentsApi.Models.Events;
using MediatR;

namespace AppointmentsApi.EventHandlers;

public class SignalRHubsAppointmentCreatedHandler : INotificationHandler<AppointmentCreatedEvent>
{
  public Task Handle(AppointmentCreatedEvent notification, CancellationToken cancellationToken)
  {
    // SignalR awesomeness here

    return Task.CompletedTask;
  }
}
