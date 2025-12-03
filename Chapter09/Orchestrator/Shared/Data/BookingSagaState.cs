using MassTransit;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Shared.Data;

public class BookingSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }  // For correlating messages

    // Additional saga data
    public Guid AppointmentId { get; set; }
    public string CurrentState { get; set; } = "None";

    // You can add more fields (e.g., Payment-related).
}
